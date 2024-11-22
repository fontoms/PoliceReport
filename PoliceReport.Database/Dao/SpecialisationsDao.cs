using PoliceReport.Core.Specialisation;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class SpecialisationsDao : ISpecialisationDao
    {
        private readonly IDatabaseConnection _connection;

        public SpecialisationsDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Specialisation specialisation)
        {
            string req = "INSERT INTO Specialisations (Nom) VALUES ('" + specialisation.Nom + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Specialisation specialisation)
        {
            string req = "DELETE FROM Specialisations WHERE Id = " + specialisation.Id;
            _connection.ExecuteNonQuery(req);
        }

        public List<Specialisation> GetAll()
        {
            string req = "SELECT * FROM Specialisations";
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
            string req = "UPDATE Specialisations SET Nom = '" + specialisation.Nom + "' WHERE Id = " + specialisation.Id;
            _connection.ExecuteNonQuery(req);
        }
    }
}
