using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net;

namespace StorageLayer
{
    public class BaseDao
    {
        private SQLiteConnection connection;
        public event EventHandler<double> ProgressChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseDao()
        {
            // chemin absolu vers la base de données 
            string localDbFolderPath = Environment.CurrentDirectory;
            string[] localDbFiles = Directory.GetFiles(localDbFolderPath, "PR_BDD_v*.db");
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

        public async Task<string> Update()
        {
            try
            {
                // Récupère la version du fichier local
                string localVersion = GetLocalDatabaseVersion();

                // Récupère la liste des fichiers dans le dossier GitHub
                string githubFilesUrl = "https://api.github.com/repos/Fontom71/PoliceReport/contents/StorageLayer";
                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "request");
                string githubFilesJson = await client.DownloadStringTaskAsync(githubFilesUrl);
                JArray githubFilesArray = JArray.Parse(githubFilesJson);

                foreach (JObject file in githubFilesArray)
                {
                    string fileName = file["name"].ToString();
                    if (fileName.Contains("PR_BDD_v") && fileName.EndsWith(".db"))
                    {
                        string[] fileNameParts = fileName.Split('_', '.');
                        string fileVersionStr = fileNameParts[2]; // Assuming the version is at index 2
                        Version fileVersion = new Version(fileVersionStr);

                        // Comparaison des versions
                        Version local = new Version(localVersion);
                        if (fileVersion > local)
                        {
                            // Télécharger le fichier depuis GitHub
                            string fileUrl = file["download_url"].ToString();
                            client.DownloadProgressChanged += (sender, e) => OnProgressChanged(e.ProgressPercentage);
                            await client.DownloadFileTaskAsync(fileUrl, Environment.CurrentDirectory + "\\" + fileName);

                            return fileVersion.ToString(); // Version mise à jour
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la mise à jour : " + ex.Message);
            }

            return null; // Aucune mise à jour effectuée
        }

        private string GetLocalDatabaseVersion()
        {
            string localDbFolderPath = Environment.CurrentDirectory;
            string[] localDbFiles = Directory.GetFiles(localDbFolderPath, "PR_BDD_v*.db");

            if (localDbFiles.Length > 0)
            {
                string latestFile = localDbFiles.OrderByDescending(f => f).FirstOrDefault();
                if (latestFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(latestFile);
                    string[] fileNameParts = fileName.Split('_', '.');
                    if (fileNameParts.Length > 1)
                    {
                        return fileNameParts[2]; // Assuming version is at index 2
                    }
                }
            }

            return "0.0"; // Version par défaut si non trouvée
        }

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
    }
}
