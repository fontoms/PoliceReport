using System.Data.SQLite;

namespace PoliceReport.Database
{
    public class RemoteDao : IDatabaseConnection
    {
        private SQLiteConnection? _connection;
        private readonly string _url;
        private readonly string _username;
        private readonly string _password;

        public RemoteDao(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }

        public void CloseConnection()
        {
            _connection?.Close();
        }

        public void Connect()
        {
            _connection ??= new SQLiteConnection("Data Source=" + _url + ";Version=3;User Id=" + _username + ";Password=" + _password + ";");
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
            string req = "SELECT name FROM sqlite_master WHERE type='table'";
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
