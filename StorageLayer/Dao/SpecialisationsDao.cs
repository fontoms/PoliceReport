using LogicLayer.Specialisation;

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
            var req = "INSERT INTO Specialisations (Nom, Type) VALUES ('" + specialisation.Nom + "', '" + specialisation.Type + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Specialisation specialisation)
        {
            var req = "DELETE FROM Specialisations WHERE Id = " + specialisation.Id;
            ExecuteNonQuery(req);
        }

        public List<Specialisation> GetAll()
        {
            var req = "SELECT * FROM Specialisations";
            var reader = ExecuteReader(req);
            var specialisations = new List<Specialisation>();
            while (reader.Read())
            {
                specialisations.Add(new Specialisation(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            CloseConnection();
            return specialisations;
        }

        public void Update(Specialisation specialisation)
        {
            var req = "UPDATE Specialisations SET Nom = '" + specialisation.Nom + "', Type = '" + specialisation.Type + "' WHERE Id = " + specialisation.Id;
            ExecuteNonQuery(req);
        }
    }
}
