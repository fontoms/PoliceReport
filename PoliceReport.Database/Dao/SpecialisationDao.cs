using PoliceReport.Core.Specialisation;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class SpecialisationDao : ISpecialisationDao
    {
        private readonly IDatabaseConnection _connection;

        public SpecialisationDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Specialisation specialisation)
        {
            string req = "INSERT INTO Specialisation (Nom) VALUES ('" + specialisation.Nom + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Specialisation specialisation)
        {
            string req = "DELETE FROM Specialisation WHERE Id = " + specialisation.Id;
            _connection.ExecuteNonQuery(req);
        }

        public List<Specialisation> GetAll()
        {
            string req = "SELECT * FROM Specialisation";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Specialisation> specialisations = [];
            while (reader.Read())
            {
                specialisations.Add(new Specialisation(reader.GetInt16(0), reader.GetString(1)));
            }
            reader.Close();
            _connection.CloseConnection();
            return specialisations;
        }

        public void Update(Specialisation specialisation)
        {
            string req = "UPDATE Specialisation SET Nom = '" + specialisation.Nom + "' WHERE Id = " + specialisation.Id;
            _connection.ExecuteNonQuery(req);
        }
    }
}
