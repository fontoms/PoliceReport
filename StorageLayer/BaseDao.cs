using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.Net;
using System.Reflection;

namespace StorageLayer
{
    public class BaseDao
    {
        private SQLiteConnection connection;
        public event EventHandler<double> ProgressChanged;
#if DEBUG
        private string githubToken = "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";
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseDao()
        {
            // chemin absolu vers la base de données 
            string localDbFolderPath = Environment.CurrentDirectory;
            string[] localDbFiles = Directory.GetFiles(localDbFolderPath, "PR_BDD_*.db");
            var path = localDbFiles.OrderByDescending(f => f).FirstOrDefault();
            // chaîne de connexion
            var connectionString = "Data Source=" + path + ";Version=3;";
            // création de la connexion
            connection = new SQLiteConnection(connectionString);
        }

        protected virtual void OnProgressChanged(double progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

        #region Mise à jour
        public async Task<bool> CheckIfUpdated()
        {
            try
            {
                // Récupère la version du fichier local et ses dates
                (string version, DateTime localCreateDate, DateTime localModifiedDate) localFile = GetLocalDatabaseVersion();

                // URL pour récupérer les détails du dernier commit
                string githubCommitUrl = $"https://api.github.com/repos/Fontom71/{AppDomain.CurrentDomain.FriendlyName}/commits";
                WebClient client = new WebClient();
#if DEBUG
                client.Headers.Add("Authorization", "Bearer " + githubToken);
#endif
                client.Headers.Add("User-Agent", "request");
                string githubCommitJson = await client.DownloadStringTaskAsync(githubCommitUrl);
                client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                JArray githubCommitsArray = JArray.Parse(githubCommitJson);

                if (githubCommitsArray.Count > 0)
                {
                    JObject latestCommit = (JObject)githubCommitsArray[0];
                    string commitSha = latestCommit["sha"].ToString();

                    // URL pour récupérer les détails du commit avec les fichiers modifiés
                    string commitDetailsUrl = $"https://api.github.com/repos/Fontom71/{AppDomain.CurrentDomain.FriendlyName}/commits/{commitSha}";
                    client.Headers.Add("User-Agent", "request");
                    string commitDetailsJson = await client.DownloadStringTaskAsync(commitDetailsUrl);
                    client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                    JObject commitDetails = JObject.Parse(commitDetailsJson);

                    JArray filesArray = (JArray)commitDetails["files"];

                    foreach (JObject file in filesArray)
                    {
                        string remoteName = Path.GetFileNameWithoutExtension(file["filename"].ToString());

                        // Vérifie si le fichier est une base de données avec le bon format de nom
                        if (remoteName.Contains("PR_BDD_"))
                        {
                            // Sépare les parties du nom de fichier pour obtenir la version
                            string[] remoteNameParts = remoteName.Split('_', '.');
                            string remoteVersionStr = string.Join(".", remoteNameParts.Skip(2));

                            // Convertit la version en objet Version
                            Version remoteVersion = new Version(remoteVersionStr);

                            // Convertit la version locale en objet Version pour la comparaison
                            Version localVersion = new Version(localFile.version);

                            // Compare les versions
                            if (remoteVersion > localVersion)
                            {
                                return true; // Une mise à jour est disponible
                            }

                            // Compare la date du commit
                            DateTime remoteCommitDate = commitDetails["commit"]["committer"]["date"].ToObject<DateTime>();

                            // Compare les dates pour mise à jour si nécessaire
                            if (localFile.localCreateDate < remoteCommitDate || localFile.localModifiedDate > remoteCommitDate)
                            {
                                return true; // Une mise à jour est disponible
                            }
                        }
                    }
                }

                return false; // Aucune mise à jour disponible
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la vérification de la mise à jour : " + ex.Message);
                return false;
            }
        }

        public async Task<string> Update()
        {
            try
            {
                // Vérifie d'abord s'il y a une mise à jour disponible
                bool updateAvailable = await CheckIfUpdated();

                if (updateAvailable)
                {
                    // Récupère la liste des fichiers dans le dossier GitHub
                    string githubFilesUrl = $"https://api.github.com/repos/Fontom71/{AppDomain.CurrentDomain.FriendlyName}/contents/{Assembly.GetExecutingAssembly().GetName().Name}";
                    WebClient client = new WebClient();
#if DEBUG
                    client.Headers.Add("Authorization", "Bearer " + githubToken);
#endif
                    client.Headers.Add("User-Agent", "request");
                    string githubFilesJson = await client.DownloadStringTaskAsync(githubFilesUrl);
                    JArray githubFilesArray = JArray.Parse(githubFilesJson);

                    foreach (JObject file in githubFilesArray)
                    {
                        // Récupère le nom du fichier
                        string fileName = Path.GetFileNameWithoutExtension(file["name"].ToString());
                        string fileNameWithExtension = file["name"].ToString();

                        // Vérifie si le fichier est une base de données avec le bon format de nom
                        if (fileName.Contains("PR_BDD_"))
                        {
                            // Sépare les parties du nom de fichier pour obtenir la version
                            string[] fileNameParts = fileName.Split('_', '.');
                            string fileVersionStr = string.Join(".", fileNameParts.Skip(2));

                            Version fileVersion = new Version(fileVersionStr);

                            // Supprime l'ancienne base de données
                            string[] localDbFiles = Directory.GetFiles(Environment.CurrentDirectory, "PR_BDD_*.db");
                            string firstFile = localDbFiles.OrderByDescending(f => f).LastOrDefault();
                            if (firstFile != null)
                            {
                                File.Delete(firstFile);
                            }

                            // Télécharge le fichier depuis GitHub
                            string fileUrl = file["download_url"].ToString();
                            client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                            await client.DownloadFileTaskAsync(fileUrl, Environment.CurrentDirectory + "\\" + fileNameWithExtension);

                            // Retourne la version mise à jour
                            return fileVersion.ToString();
                        }
                    }
                }
                else
                {
                    // Aucune mise à jour disponible
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la mise à jour : " + ex.Message);
            }

            return null; // Aucune mise à jour effectuée
        }

        private (string version, DateTime localCreateDate, DateTime localModifiedDate) GetLocalDatabaseVersion()
        {
            string localDbFolderPath = Environment.CurrentDirectory;
            string[] localDbFiles = Directory.GetFiles(localDbFolderPath, "PR_BDD_*.db");

            if (localDbFiles.Length > 0)
            {
                string latestFile = localDbFiles.OrderByDescending(f => f).FirstOrDefault();
                if (latestFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(latestFile);
                    FileInfo fileInfo = new FileInfo(latestFile);
                    string[] fileNameParts = fileName.Split('_', '.');
                    if (fileNameParts.Length > 2)
                    {
                        string fileVersionStr = string.Join(".", fileNameParts.Skip(2));
                        return (version: fileVersionStr, localCreateDate: fileInfo.CreationTime, localModifiedDate: fileInfo.LastWriteTime);
                    }
                }
            }

            return ("0.0.0", DateTime.MinValue, DateTime.MinValue);
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Open the connection
        /// </summary>
        /// <param name="req">SQLiteCommand</param>
        public void ExecuteNonQuery(string req)
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = req;
            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// Execute a query
        /// </summary>
        /// <param name="req">SQLiteCommand</param>
        /// <returns>SQLiteDataReader</returns>
        public SQLiteDataReader ExecuteReader(string req)
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = req;
            var reader = command.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void CloseConnection()
        {
            connection.Close();
        }

        public List<string> GetTables()
        {
            List<string> tables = new List<string>();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }
            connection.Close();
            return tables;
        }

        public List<string> GetColumnsOfTable(string tableName)
        {
            List<string> columns = new List<string>();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "PRAGMA table_info(" + tableName + ")";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(reader.GetString(1));
            }
            connection.Close();
            return columns;
        }
        #endregion
    }
}
