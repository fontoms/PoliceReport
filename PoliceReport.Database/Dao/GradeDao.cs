using PoliceReport.Core.Grade;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class GradeDao : IGradeDao
    {
        private readonly IDatabaseConnection _connection;

        public GradeDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Grade grade)
        {
            string req = "INSERT INTO Grade (Nom) VALUES ('" + grade.Nom + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Grade grade)
        {
            string req = "DELETE FROM Grade WHERE Id = " + grade.Id;
            _connection.ExecuteNonQuery(req);
        }

        public List<Grade> GetAll()
        {
            string req = "SELECT * FROM Grade";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Grade> grades = [];
            while (reader.Read())
            {
                grades.Add(new Grade(reader.GetInt16(0), reader.GetString(1)));
            }
            reader.Close();
            _connection.CloseConnection();
            return grades;
        }

        public void Update(Grade grade)
        {
            string req = "UPDATE Grade SET Nom = '" + grade.Nom + "' WHERE Id = " + grade.Id;
            _connection.ExecuteNonQuery(req);
        }
    }
}
