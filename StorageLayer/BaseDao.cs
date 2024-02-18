using System.Data.SQLite;

namespace StorageLayer
{
    public class BaseDao
    {
        private SQLiteConnection connection;

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
    }
}
