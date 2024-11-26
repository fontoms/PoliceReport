using PoliceReport.Database;

namespace PoliceReport.Tests.Database
{
    public class LocalDaoTests
    {
        private readonly IDatabaseConnection localDao;

        public LocalDaoTests()
        {
            localDao = LocalDao.Instance;
            localDao.Connect();
        }

        [Fact]
        public void TestConnectionToLocalDatabase()
        {
            var exception = Record.Exception(() => localDao.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TestTable (Id INTEGER PRIMARY KEY, Name TEXT)"));
            Assert.Null(exception);
        }

        [Fact]
        public void TestInsertAndSelect()
        {
            localDao.ExecuteNonQuery("INSERT INTO TestTable (Name) VALUES ('TestName')");
            var reader = localDao.ExecuteReader("SELECT Name FROM TestTable WHERE Name = 'TestName'");
            Assert.True(reader.Read());
            Assert.Equal("TestName", reader.GetString(0));
            reader.Close();
        }

        public void Dispose()
        {
            localDao.ExecuteNonQuery("DROP TABLE IF EXISTS TestTable");
            localDao.CloseConnection();
        }
    }
}
