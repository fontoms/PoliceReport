using System.Data.SQLite;

namespace StorageLayer
{
    public class StorageDao : BaseDao
    {
        private readonly SQLiteConnection connection;

        public StorageDao(string fileName) : base()
        {
            connection = new SQLiteConnection(@"DataSource=" + fileName);
        }

        public void ExecuteNonQuery(string req)
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = req;
            command.ExecuteNonQuery();
            connection.Close();
        }

        public SQLiteDataReader ExecuteReader(string req)
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = req;
            var reader = command.ExecuteReader();
            return reader;
        }
    }
}
