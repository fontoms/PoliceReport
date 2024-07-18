using LogicLayer.Specialisation;
using System.Data.SQLite;

namespace StorageLayer.Dao
{
    public class SpecialisationsDao : BaseDao, ISpecialisationDao
    {
        private static SpecialisationsDao? _instance;

        private SpecialisationsDao() : base() { }

        public static SpecialisationsDao Instance
        {
            get
            {
                _instance ??= new SpecialisationsDao();
                return _instance;
            }
        }

        public void Add(Specialisation specialisation)
        {
            string req = "INSERT INTO Specialisations (Nom) VALUES ('" + specialisation.Nom + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Specialisation specialisation)
        {
            string req = "DELETE FROM Specialisations WHERE Id = " + specialisation.Id;
            ExecuteNonQuery(req);
        }

        public List<Specialisation> GetAll()
        {
            string req = "SELECT * FROM Specialisations";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Specialisation> specialisations = [];
            while (reader.Read())
            {
                specialisations.Add(new Specialisation(reader.GetInt16(0), reader.GetString(1)));
            }
            reader.Close();
            CloseConnection();
            return specialisations;
        }

        public void Update(Specialisation specialisation)
        {
            string req = "UPDATE Specialisations SET Nom = '" + specialisation.Nom + "' WHERE Id = " + specialisation.Id;
            ExecuteNonQuery(req);
        }
    }
}
