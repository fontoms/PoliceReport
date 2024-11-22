using PoliceReport.Core.Utilisateur;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class UtilisateursDao : IUtilisateurDao
    {
        private readonly IDatabaseConnection _connection;

        public UtilisateursDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Utilisateur user)
        {
            string req = "INSERT INTO Utilisateurs (Username, Password, Role) VALUES ('" + user.Username + "', '" + user.Password + "', '" + user.Role + "')";
            _connection.ExecuteNonQuery(req);
        }

        public List<Utilisateur> GetAll()
        {
            string req = "SELECT * FROM Utilisateurs";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<Utilisateur> users = new List<Utilisateur>();
            while (reader.Read())
            {
                users.Add(new Utilisateur(reader.GetInt16(0), reader.GetString(1), reader.GetString(2), reader.GetInt16(3)));
            }
            reader.Close();
            _connection.CloseConnection();
            return users;
        }

        public Utilisateur GetUser(string username, string password)
        {
            string req = "SELECT * FROM Utilisateurs WHERE Username = '" + username + "' AND Password = '" + password + "'";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            if (reader.Read())
            {
                Utilisateur user = new Utilisateur(reader.GetString(1), reader.GetString(2), reader.GetInt16(3));
                reader.Close();
                _connection.CloseConnection();
                return user;
            }
            else
            {
                reader.Close();
                _connection.CloseConnection();
                return null;
            }
        }

        public void Remove(Utilisateur user)
        {
            string req = "DELETE FROM Utilisateurs WHERE Id = " + user.Id;
            _connection.ExecuteNonQuery(req);
        }

        public void Update(Utilisateur user)
        {
            string req = "UPDATE Utilisateurs SET Username = '" + user.Username + "', Password = '" + user.Password + "', Role = '" + user.Role + "' WHERE Id = " + user.Id;
            _connection.ExecuteNonQuery(req);
        }
    }
}
