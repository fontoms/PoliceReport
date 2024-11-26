using System.Data.SQLite;

namespace PoliceReport.Database
{
    public interface IDatabaseConnection
    {
        void Connect();
        void CloseConnection();
        SQLiteDataReader ExecuteReader(string query);
        void ExecuteNonQuery(string query);
        List<string> GetTables();
        List<(string Name, Type Type)> GetColumnsOfTable(string tableName);
    }
}
