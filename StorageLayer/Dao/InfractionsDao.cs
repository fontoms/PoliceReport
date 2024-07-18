using LogicLayer.Infraction;
using System.Data.SQLite;

namespace StorageLayer.Dao
{
    public class InfractionsDao : BaseDao, IInfractionDao
    {
        private static InfractionsDao? _instance;

        private InfractionsDao() : base() { }

        public static InfractionsDao Instance
        {
            get
            {
                _instance ??= new InfractionsDao();
                return _instance;
            }
        }

        public void Add(Infraction infraction)
        {
            string req = "INSERT INTO Infractions (Nom) VALUES ('" + infraction.Nom + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Infraction infraction)
        {
            string req = "DELETE FROM Infractions WHERE Id = " + infraction.Id;
            ExecuteNonQuery(req);
        }

        public List<Infraction> GetAll()
        {
            string req = "SELECT * FROM Infractions";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Infraction> infractions = [];
            while (reader.Read())
            {
                infractions.Add(new Infraction(reader.GetInt16(0), reader.GetString(1)));
            }
            reader.Close();
            CloseConnection();
            return infractions;
        }

        public void Update(Infraction infraction)
        {
            string req = "UPDATE Infractions SET Nom = '" + infraction.Nom + "' WHERE Id = " + infraction.Id;
            ExecuteNonQuery(req);
        }
    }
}
