using LogicLayer.Vehicule;

namespace StorageLayer.Dao
{
    public class VehiculesDao : BaseDao, IVehiculeDao
    {
        public void Add(Vehicule vehicule)
        {
            throw new NotImplementedException();
        }

        public void Remove(Vehicule vehicule)
        {
            throw new NotImplementedException();
        }

        public List<Vehicule> GetAll()
        {
            var req = "SELECT * FROM Vehicules ORDER BY Nom";
            var reader = ExecuteReader(req);
            var vehicules = new List<Vehicule>();
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            return vehicules;
        }

        public List<Vehicule> GetAllBySpecialisation(string specialisation)
        {
            var req = "SELECT * FROM Vehicules WHERE VehSpe = '" + specialisation + "' ORDER BY Nom";
            var reader = ExecuteReader(req);
            var vehicules = new List<Vehicule>();
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
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
            return vehicules;
        }
    }
}
