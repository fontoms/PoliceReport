using LogicLayer.Unite;

namespace StorageLayer.Dao
{
    public class UnitesDao : BaseDao, IUniteDao
    {
        public UnitesDao() : base() { }

        public void Add(Unite unite)
        {
            throw new NotImplementedException();
        }

        public void Remove(Unite unite)
        {
            throw new NotImplementedException();
        }

        public List<Unite> GetAll()
        {
            var req = "SELECT * FROM Unites";
            var reader = ExecuteReader(req);
            var unites = new List<Unite>();
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            reader.Close();
            return unites;
        }

        public List<Unite> GetAllBySpecialisation(string specialisation)
        {
            var req = "SELECT * FROM Unites WHERE UnitSpe = '" + specialisation + "' ORDER BY UnitSpe ASC";
            var reader = ExecuteReader(req);
            var unites = new List<Unite>();
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            reader.Close();
            return unites;
        }

        public List<Unite> GetAllByNom(string nom)
        {
            var req = "SELECT * FROM Unites WHERE Nom = '" + nom + "' ORDER BY Nom ASC";
            var reader = ExecuteReader(req);
            var unites = new List<Unite>();
            while (reader.Read())
            {
                unites.Add(new Unite(reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            reader.Close();
            return unites;
        }
    }
}
