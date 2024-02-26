using System.Data.SQLite;

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
            var path = Environment.CurrentDirectory + "\\PRBaseDeDonnee.db";
            // chaîne de connexion
            var connectionString = "Data Source=" + path + ";Version=3;";
            // création de la connexion
            connection = new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// Crée les tables de la base de données
        /// </summary>
        public void CreateTables()
        {
            // Lis le fichier PRBaseDeDonnee.db.sql et exécute les commandes SQL
            var path = Environment.CurrentDirectory + "\\Installation.sql";
            var sql = File.ReadAllText(path);
            var commands = sql.Split(new string[] { ";\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            double progressStep = 100.0 / commands.Length;
            double currentProgress = 0;

            foreach (var command in commands)
            {
                // Ignore BEGIN TRANSACTION et COMMIT
                if (command.Trim().Length > 0 && !command.Trim().StartsWith("BEGIN") && !command.Trim().StartsWith("COMMIT"))
                {
                    ExecuteNonQuery(command);
                }

                currentProgress += progressStep;
                // Envoyer l'événement de progression
                OnProgressChanged(currentProgress);
            }
        }

        protected virtual void OnProgressChanged(double progress)
        {
            ProgressChanged?.Invoke(this, progress);
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
