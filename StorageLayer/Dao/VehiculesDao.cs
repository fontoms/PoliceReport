using LogicLayer.Vehicule;

namespace StorageLayer.Dao
{
    public class VehiculesDao : BaseDao, IVehiculeDao
    {
        private static VehiculesDao? _instance;

        public static VehiculesDao Instance
        {
            get
            {
                _instance ??= new VehiculesDao();
                return _instance;
            }
        }

        public void Add(Vehicule vehicule)
        {
            var req = "INSERT INTO Vehicules (Nom, VehSpecialisation) VALUES ('" + vehicule.Nom + "', '" + vehicule.VehSpecialisation + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Vehicule vehicule)
        {
            var req = "DELETE FROM Vehicules WHERE Id = " + vehicule.Id;
            ExecuteNonQuery(req);
        }

        public List<Vehicule> GetAll()
        {
            var req = "SELECT * FROM Vehicules";
            var reader = ExecuteReader(req);
            var vehicules = new List<Vehicule>();
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            CloseConnection();
            return vehicules;
        }

        public List<Vehicule> GetAllBySpecialisation(string specialisation)
        {
            var req = "SELECT * FROM Vehicules WHERE VehSpecialisation = '" + specialisation + "' ORDER BY Nom";
            var reader = ExecuteReader(req);
            var vehicules = new List<Vehicule>();
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            CloseConnection();
            return vehicules;
        }

        public List<Vehicule> GetAllByNom(string nom)
        {
            var req = "SELECT * FROM Vehicules WHERE Nom = '" + nom + "' ORDER BY Nom";
            var reader = ExecuteReader(req);
            var vehicules = new List<Vehicule>();
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            CloseConnection();
            return vehicules;
        }

        public List<Vehicule> GetAllByNameContains(string name)
        {
            var req = "SELECT * FROM Vehicules WHERE Nom LIKE '%" + name + "%' ORDER BY Nom";
            var reader = ExecuteReader(req);
            var vehicules = new List<Vehicule>();
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            CloseConnection();
            return vehicules;
        }

        public void Update(Vehicule vehicule)
        {
            var req = "UPDATE Vehicules SET Nom = '" + vehicule.Nom + "', VehSpecialisation = '" + vehicule.VehSpecialisation + "' WHERE Id = " + vehicule.Id;
            ExecuteNonQuery(req);
        }
    }
}
