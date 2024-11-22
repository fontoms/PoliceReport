using PoliceReport.Core.Infraction;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class InfractionsDao : IInfractionDao
    {
        private readonly IDatabaseConnection _connection;

        public InfractionsDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Infraction infraction)
        {
            string req = "INSERT INTO Infractions (Nom) VALUES ('" + infraction.Nom + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Infraction infraction)
        {
            string req = "DELETE FROM Infractions WHERE Id = " + infraction.Id;
            _connection.ExecuteNonQuery(req);
        }

        public List<Infraction> GetAll()
        {
            string req = "SELECT * FROM Infractions";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Infraction> infractions = [];
            while (reader.Read())
            {
                infractions.Add(new Infraction(reader.GetInt16(0), reader.GetString(1)));
            }
            reader.Close();
            _connection.CloseConnection();
            return infractions;
        }

        public void Update(Infraction infraction)
        {
            string req = "UPDATE Infractions SET Nom = '" + infraction.Nom + "' WHERE Id = " + infraction.Id;
            _connection.ExecuteNonQuery(req);
        }
    }
}
