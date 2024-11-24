using PoliceReport.Core.Vehicule;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class VehiculeDao : IVehiculeDao
    {
        private readonly IDatabaseConnection _connection;

        public VehiculeDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Vehicule vehicule)
        {
            string req = "INSERT INTO Vehicule (Nom, VehSpecialisation) VALUES ('" + vehicule.Nom + "', '" + vehicule.VehSpecialisation + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Vehicule vehicule)
        {
            string req = "DELETE FROM Vehicule WHERE Id = " + vehicule.Id;
            _connection.ExecuteNonQuery(req);
        }

        public List<Vehicule> GetAll()
        {
            string req = "SELECT * FROM Vehicule";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Vehicule> vehicules = [];
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetInt16(0), reader.GetInt16(1), reader.GetString(2)));
            }
            reader.Close();
            _connection.CloseConnection();
            return vehicules;
        }

        public List<Vehicule> GetAllBySpecialisation(int specialisation)
        {
            string req = "SELECT * FROM Vehicule WHERE VehSpecialisation = '" + specialisation + "' ORDER BY Nom";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Vehicule> vehicules = [];
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetInt16(1), reader.GetString(2)));
            }
            reader.Close();
            _connection.CloseConnection();
            return vehicules;
        }

        public List<Vehicule> GetAllByNom(string nom)
        {
            string req = "SELECT * FROM Vehicule WHERE Nom = '" + nom + "' ORDER BY Nom";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Vehicule> vehicules = [];
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetInt16(1), reader.GetString(2)));
            }
            reader.Close();
            _connection.CloseConnection();
            return vehicules;
        }

        public List<Vehicule> GetAllByNameContains(string name)
        {
            string req = "SELECT * FROM Vehicule WHERE Nom LIKE '%" + name + "%' ORDER BY Nom";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Vehicule> vehicules = [];
            while (reader.Read())
            {
                vehicules.Add(new Vehicule(reader.GetInt16(1), reader.GetString(2)));
            }
            reader.Close();
            _connection.CloseConnection();
            return vehicules;
        }

        public void Update(Vehicule vehicule)
        {
            string req = "UPDATE Vehicule SET Nom = '" + vehicule.Nom + "', VehSpecialisation = '" + vehicule.VehSpecialisation + "' WHERE Id = " + vehicule.Id;
            _connection.ExecuteNonQuery(req);
        }
    }
}
