using PoliceReport.Core.Unite;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class UnitesDao : IUniteDao
    {
        private readonly IDatabaseConnection _connection;

        public UnitesDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Unite unite)
        {
            string req = "INSERT INTO Unites (Nom, UnitSpecialisation) VALUES ('" + unite.Nom + "', '" + unite.UnitSpecialisation + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Unite unite)
        {
            string req = "DELETE FROM Unites WHERE Id = " + unite.Id;
            _connection.ExecuteNonQuery(req);
        }

        public Unite GetId(int id)
        {
            string req = "SELECT * FROM Unites WHERE Id = '" + id + "'";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            reader.Read();
            Unite unite = new Unite(reader.GetString(1), reader.GetInt16(2));
            reader.Close();
            _connection.CloseConnection();
            return unite;
        }

        public Unite GetType(string type)
        {
            string req = "SELECT * FROM Unites WHERE Type = '" + type + "'";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            reader.Read();
            Unite unite = new Unite(reader.GetString(1), reader.GetInt16(2));
            reader.Close();
            _connection.CloseConnection();
            return unite;
        }

        public List<Unite> GetAll()
        {
            string req = "SELECT * FROM Unites";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Unite> unites = [];
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetInt16(0), reader.GetString(1), reader.GetInt16(2)));
            }
            reader.Close();
            _connection.CloseConnection();
            return unites;
        }

        public List<Unite> GetAllBySpecialisation(int specialisation)
        {
            string req = "SELECT * FROM Unites WHERE UnitSpecialisation = '" + specialisation + "' ORDER BY UnitSpecialisation ASC";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Unite> unites = [];
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetInt16(0), reader.GetString(1), reader.GetInt16(2)));
            }
            reader.Close();
            _connection.CloseConnection();
            return unites;
        }

        public List<Unite> GetAllByNom(string nom)
        {
            string req = "SELECT * FROM Unites WHERE Nom = '" + nom + "' ORDER BY Nom ASC";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Unite> unites = [];
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetString(1), reader.GetInt16(2)));
            }
            reader.Close();
            _connection.CloseConnection();
            return unites;
        }

        public void Update(Unite unite)
        {
            string req = "UPDATE Unites SET Nom = '" + unite.Nom + "', UnitSpecialisation = '" + unite.UnitSpecialisation + "' WHERE Id = " + unite.Id;
            _connection.ExecuteNonQuery(req);
        }
    }
}
