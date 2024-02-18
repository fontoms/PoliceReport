using LogicLayer.Grade;

namespace StorageLayer.Dao
{
    public class GradesDao : BaseDao, IGradeDao
    {
        public GradesDao() : base() { }

        public void Add(Grade grade)
        {
            throw new NotImplementedException();
        }

        public void Remove(Grade grade)
        {
            throw new NotImplementedException();
        }

        public List<Grade> GetAll()
        {
            var req = "SELECT * FROM Grades";
            var reader = ExecuteReader(req);
            var grades = new List<Grade>();
            while (reader.Read())
            {
                grades.Add(new Grade(reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            return grades;
        }
    }
}
