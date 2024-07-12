using LogicLayer.Grade;
using System.Data.SQLite;

namespace StorageLayer.Dao
{
    public class GradesDao : BaseDao, IGradeDao
    {
        private static GradesDao? _instance;

        private GradesDao() : base() { }

        public static GradesDao Instance
        {
            get
            {
                _instance ??= new GradesDao();
                return _instance;
            }
        }

        public void Add(Grade grade)
        {
            string req = "INSERT INTO Grades (Type, Nom) VALUES ('" + grade.Type + "', '" + grade.Nom + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Grade grade)
        {
            string req = "DELETE FROM Grades WHERE Id = " + grade.Id;
            ExecuteNonQuery(req);
        }

        public List<Grade> GetAll()
        {
            string req = "SELECT * FROM Grades";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Grade> grades = [];
            while (reader.Read())
            {
                grades.Add(new Grade(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            CloseConnection();
            return grades;
        }

        public void Update(Grade grade)
        {
            string req = "UPDATE Grades SET Type = '" + grade.Type + "', Nom = '" + grade.Nom + "' WHERE Id = " + grade.Id;
            ExecuteNonQuery(req);
        }
    }
}
