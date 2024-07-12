using LogicLayer.Outils;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.Net;

namespace StorageLayer
{
    public class BaseDao
    {
        private SQLiteConnection connection;
        private string githubContentsUrl = Constants.ApiContentUrl;
        public event EventHandler<double> ProgressChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseDao()
        {
            // chemin absolu vers la base de données 
            string localDbFolderPath = Environment.CurrentDirectory;
            string[] localDbFiles = Directory.GetFiles(localDbFolderPath, "PR_BDD_*.db");
            var path = localDbFiles.OrderByDescending(f => f).FirstOrDefault();
            if (path != null)
            {
                // chaîne de connexion
                var connectionString = "Data Source=" + path + ";Version=3;";
                // création de la connexion
                connection = new SQLiteConnection(connectionString);
            }
            else
            {
                connection = new SQLiteConnection("Data Source=:memory:;Version=3;");
            }
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

                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "request");
                client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                string githubContentsJson = await client.DownloadStringTaskAsync(githubContentsUrl);
                JArray githubContentsArray = JArray.Parse(githubContentsJson);

                foreach (JObject content in githubContentsArray)
                {
                    string remoteName = content["name"].ToString();

                    // Vérifie si le fichier est une base de données avec le bon format de nom
                    if (remoteName.StartsWith("PR_BDD_") && remoteName.EndsWith(".db"))
                    {
                        // Extrait la version du nom de fichier (format attendu : PR_BDD_x.x.x.db)
                        string[] nameParts = remoteName.Split('_', '.');
                        if (nameParts.Length >= 4)
                        {
                            string remoteVersionStr = $"{nameParts[2]}.{nameParts[3]}.{nameParts[4]}";
                            Version remoteVersion = new Version(remoteVersionStr);

                            // Convertit la version locale en objet Version pour la comparaison
                            Version localVersion = new Version(localFile.version);

                            OnProgressChanged(githubContentsArray.IndexOf(content) * 100 / githubContentsArray.Count);

                            // Compare les versions
                            if (remoteVersion > localVersion)
                            {
                                Settings.Default.LatestUpdate = DateTime.Now; // Met à jour la date de dernière mise à jour
                                Settings.Default.Save();
                                return true; // La base de données est mise à jour, donc retourne true
                            }
                        }
                    }
                }

                // Compare la date de dernière mise à jour avec la date de modification locale du fichier de BDD
                DateTime localDate = localFile.localModifiedDate.AddMinutes(-1); // Ajoute 5 minutes pour compenser le décalage horaire
                if (localDate > Settings.Default.LatestUpdate)
                {
                    Settings.Default.LatestUpdate = DateTime.Now; // Met à jour la date de dernière mise à jour
                    Settings.Default.Save();
                    return true; // La base de données est mise à jour, donc retourne true
                }

                return false; // Aucune mise à jour disponible ou nécessaire
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
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
                    WebClient client = new WebClient();
                    client.Headers.Add("User-Agent", "request");
                    client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                    string githubFilesJson = await client.DownloadStringTaskAsync(githubContentsUrl);
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

                            OnProgressChanged(githubFilesArray.IndexOf(file) * 100 / githubFilesArray.Count);

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
                throw new Exception(ex.Message, ex);
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
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = req;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                throw new Exception("Erreur avec la base de données :\n" + req);
            }
        }

        /// <summary>
        /// Execute a query
        /// </summary>
        /// <param name="req">SQLiteCommand</param>
        /// <returns>SQLiteDataReader</returns>
        public SQLiteDataReader ExecuteReader(string req)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = req;
                var reader = command.ExecuteReader();
                return reader;
            }
            catch (Exception)
            {
                throw new Exception("Erreur avec la base de données :\n" + req);
            }
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
