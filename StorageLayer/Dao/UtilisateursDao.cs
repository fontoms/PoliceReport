using LogicLayer.Utilisateur;
using System.Data.SQLite;

namespace StorageLayer.Dao
{
    public class UtilisateursDao : BaseDao, IUtilisateurDao
    {
        private static UtilisateursDao? _instance;

        private UtilisateursDao() : base() { }

        public static UtilisateursDao Instance
        {
            get
            {
                _instance ??= new UtilisateursDao();
                return _instance;
            }
        }

        public void Add(Utilisateur user)
        {
            string req = "INSERT INTO Utilisateurs (Username, Password, Role) VALUES ('" + user.Username + "', '" + user.Password + "', '" + user.Role + "')";
            ExecuteNonQuery(req);
        }

        public List<Utilisateur> GetAll()
        {
            string req = "SELECT * FROM Utilisateurs";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Utilisateur> users = new List<Utilisateur>();
            while (reader.Read())
            {
                users.Add(new Utilisateur(reader.GetInt16(0), reader.GetString(1), reader.GetString(2), reader.GetInt16(3)));
            }
            reader.Close();
            CloseConnection();
            return users;
        }

        public Utilisateur GetUser(string username, string password)
        {
            string req = "SELECT * FROM Utilisateurs WHERE Username = '" + username + "' AND Password = '" + password + "'";
            SQLiteDataReader reader = ExecuteReader(req);
            if (reader.Read())
            {
                Utilisateur user = new Utilisateur(reader.GetString(1), reader.GetString(2), reader.GetInt16(3));
                reader.Close();
                CloseConnection();
                return user;
            }
            else
            {
                reader.Close();
                CloseConnection();
                return null;
            }
        }

        public void Remove(Utilisateur user)
        {
            string req = "DELETE FROM Utilisateurs WHERE Id = " + user.Id;
            ExecuteNonQuery(req);
        }

        public void Update(Utilisateur user)
        {
            string req = "UPDATE Utilisateurs SET Username = '" + user.Username + "', Password = '" + user.Password + "', Role = '" + user.Role + "' WHERE Id = " + user.Id;
            ExecuteNonQuery(req);
        }
    }
}
