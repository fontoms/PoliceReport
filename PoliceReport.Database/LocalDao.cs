using System.Data.SQLite;
using System.Reflection;

namespace PoliceReport.Database
{
    public class LocalDao : IDatabaseConnection
    {
        private static LocalDao? _instance;
        private static readonly object _lock = new object();
        private SQLiteConnection? _connection;

        private LocalDao() { }

        public static LocalDao Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new LocalDao();
                    _instance.Connect();
                    return _instance;
                }
            }
        }

        public void CloseConnection() => _connection?.Close();

        public void Connect()
        {
            if (_connection != null) return;

            string tempDbPath = Path.Combine(Environment.CurrentDirectory, "database.db");
            if (!File.Exists(tempDbPath))
            {
                using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.database.db");
                if (stream == null) throw new Exception("Le fichier de base de données n'a pas été trouvé dans les ressources.");
                using FileStream fileStream = new FileStream(tempDbPath, FileMode.Create, FileAccess.Write);
                stream.CopyTo(fileStream);
            }

            _connection = new SQLiteConnection($"Data Source={tempDbPath};Version=3;");
        }

        public void ExecuteNonQuery(string query)
        {
            try
            {
                Connect();
                _connection.Open();
                using var command = new SQLiteCommand(query, _connection);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw new Exception($"Erreur avec la base de données :\n{query}");
            }
            finally
            {
                _connection.Close();
            }
        }

        public SQLiteDataReader ExecuteReader(string query)
        {
            try
            {
                Connect();
                _connection.Open();
                using var command = new SQLiteCommand(query, _connection);
                return command.ExecuteReader();
            }
            catch (Exception)
            {
                throw new Exception($"Erreur avec la base de données :\n{query}");
            }
        }

        public List<(string Name, Type Type)> GetColumnsOfTable(string tableName)
        {
            var columns = new List<(string Name, Type Type)>();
            using var reader = ExecuteReader($"PRAGMA table_info({tableName})");
            while (reader.Read())
            {
                string columnName = reader.GetString(1);
                string dataType = reader.GetString(2).ToLower();
                Type columnType = dataType switch
                {
                    "integer" => typeof(int),
                    "real" => typeof(double),
                    "text" => typeof(string),
                    "blob" => typeof(byte[]),
                    _ => typeof(object)
                };
                columns.Add((columnName, columnType));
            }
            CloseConnection();
            return columns;
        }

        public List<string> GetTables()
        {
            var tables = new List<string>();
            using var reader = ExecuteReader("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name");
            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }
            CloseConnection();
            return tables;
        }
    }
}
