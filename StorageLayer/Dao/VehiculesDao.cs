using LogicLayer.Vehicule;
using System.Data.SQLite;

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
            string req = "INSERT INTO Vehicules (Nom, VehSpecialisation) VALUES ('" + vehicule.Nom + "', '" + vehicule.VehSpecialisation + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Vehicule vehicule)
        {
            string req = "DELETE FROM Vehicules WHERE Id = " + vehicule.Id;
            ExecuteNonQuery(req);
        }

        public List<Vehicule> GetAll()
        {
            string req = "SELECT * FROM Vehicules";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Vehicule> vehicules = [];
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
            string req = "SELECT * FROM Vehicules WHERE VehSpecialisation = '" + specialisation + "' ORDER BY Nom";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Vehicule> vehicules = [];
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
            string req = "SELECT * FROM Vehicules WHERE Nom = '" + nom + "' ORDER BY Nom";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Vehicule> vehicules = [];
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
            string req = "SELECT * FROM Vehicules WHERE Nom LIKE '%" + name + "%' ORDER BY Nom";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Vehicule> vehicules = [];
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
            string req = "UPDATE Vehicules SET Nom = '" + vehicule.Nom + "', VehSpecialisation = '" + vehicule.VehSpecialisation + "' WHERE Id = " + vehicule.Id;
            ExecuteNonQuery(req);
        }
    }
}
