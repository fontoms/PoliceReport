using PoliceReport.Core.Grade;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class GradesDao : IGradeDao
    {
        private readonly IDatabaseConnection _connection;

        public GradesDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Grade grade)
        {
            string req = "INSERT INTO Grades (Nom) VALUES ('" + grade.Nom + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Grade grade)
        {
            string req = "DELETE FROM Grades WHERE Id = " + grade.Id;
            _connection.ExecuteNonQuery(req);
        }

        public List<Grade> GetAll()
        {
            string req = "SELECT * FROM Grades";
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
            string req = "UPDATE Grades SET Nom = '" + grade.Nom + "' WHERE Id = " + grade.Id;
            _connection.ExecuteNonQuery(req);
        }
    }
}
