using LogicLayer.Unite;
using System.Data.SQLite;

namespace StorageLayer.Dao
{
    public class UnitesDao : BaseDao, IUniteDao
    {
        private static UnitesDao? _instance;

        private UnitesDao() : base() { }

        public static UnitesDao Instance
        {
            get
            {
                _instance ??= new UnitesDao();
                return _instance;
            }
        }

        public void Add(Unite unite)
        {
            string req = "INSERT INTO Unites (Nom, Type, UnitSpecialisation) VALUES ('" + unite.Nom + "', '" + unite.Type + "', '" + unite.UnitSpecialisation + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Unite unite)
        {
            string req = "DELETE FROM Unites WHERE Id = " + unite.Id;
            ExecuteNonQuery(req);
        }

        public Unite GetType(string type)
        {
            string req = "SELECT * FROM Unites WHERE Type = '" + type + "'";
            SQLiteDataReader reader = ExecuteReader(req);
            reader.Read();
            Unite unite = new(reader.GetString(1), reader.GetString(2), reader.GetString(3));
            reader.Close();
            CloseConnection();
            return unite;
        }

        public List<Unite> GetAll()
        {
            string req = "SELECT * FROM Unites";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Unite> unites = [];
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            reader.Close();
            CloseConnection();
            return unites;
        }

        public List<Unite> GetAllBySpecialisation(string specialisation)
        {
            string req = "SELECT * FROM Unites WHERE UnitSpecialisation = '" + specialisation + "' ORDER BY UnitSpecialisation ASC";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Unite> unites = [];
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            reader.Close();
            CloseConnection();
            return unites;
        }

        public List<Unite> GetAllByNom(string nom)
        {
            string req = "SELECT * FROM Unites WHERE Nom = '" + nom + "' ORDER BY Nom ASC";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Unite> unites = [];
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            reader.Close();
            CloseConnection();
            return unites;
        }

        public void Update(Unite unite)
        {
            string req = "UPDATE Unites SET Nom = '" + unite.Nom + "', Type = '" + unite.Type + "', UnitSpecialisation = '" + unite.UnitSpecialisation + "' WHERE Id = " + unite.Id;
            ExecuteNonQuery(req);
        }
    }
}
