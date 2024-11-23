using System.Data.SQLite;

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
                    if (_instance == null)
                    {
                        _instance = new LocalDao();
                        _instance.Connect();
                    }
                    return _instance;
                }
            }
        }

        public void CloseConnection()
        {
            _connection?.Close();
        }

        public void Connect()
        {
            if (_connection == null)
            {
                string localDbFolderPath = Environment.CurrentDirectory;
                string[] localDbFiles = Directory.GetFiles(localDbFolderPath, "PR_BDD_*.db");
                var path = localDbFiles.OrderByDescending(f => f).FirstOrDefault();
                if (path != null)
                {
                    var connectionString = "Data Source=" + path + ";Version=3;";
                    _connection = new SQLiteConnection(connectionString);
                }
                else
                {
                    _connection = new SQLiteConnection("Data Source=:memory:;Version=3;");
                }
            }
        }

        public void ExecuteNonQuery(string query)
        {
            try
            {
                if (_connection == null)
                {
                    Connect();
                }
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
                _connection.Close();
            }
            catch (Exception)
            {
                throw new Exception("Erreur avec la base de données :\n" + query);
            }
        }

        public SQLiteDataReader ExecuteReader(string query)
        {
            try
            {
                if (_connection == null)
                {
                    Connect();
                }
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = query;
                var reader = command.ExecuteReader();
                return reader;
            }
            catch (Exception)
            {
                throw new Exception("Erreur avec la base de données :\n" + query);
            }
        }

        public List<(string Name, Type Type)> GetColumnsOfTable(string tableName)
        {
            List<(string Name, Type Type)> columns = new List<(string Name, Type Type)>();
            string req = "PRAGMA table_info(" + tableName + ")";
            SQLiteDataReader reader = ExecuteReader(req);
            while (reader.Read())
            {
                columns.Add((reader.GetString(1), Type.GetType(reader.GetString(2))));
            }
            reader.Close();
            CloseConnection();
            return columns;
        }

        public List<string> GetTables()
        {
            List<string> tables = new List<string>();
            string req = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name";
            SQLiteDataReader reader = ExecuteReader(req);
            while (reader.Read())
            {
                tables.Add(reader.GetString(0));
            }
            reader.Close();
            CloseConnection();
            return tables;
        }
    }
}
