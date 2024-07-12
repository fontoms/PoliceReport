using LogicLayer.Infraction;

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
            var req = "INSERT INTO Infractions (Nom, Type) VALUES ('" + infraction.Nom + "', '" + infraction.Type + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Infraction infraction)
        {
            var req = "DELETE FROM Infractions WHERE Id = " + infraction.Id;
            ExecuteNonQuery(req);
        }

        public List<Infraction> GetAll()
        {
            var req = "SELECT * FROM Infractions";
            var reader = ExecuteReader(req);
            var infractions = new List<Infraction>();
            while (reader.Read())
            {
                infractions.Add(new Infraction(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            CloseConnection();
            return infractions;
        }

        public void Update(Infraction infraction)
        {
            var req = "UPDATE Infractions SET Nom = '" + infraction.Nom + "', Type = '" + infraction.Type + "' WHERE Id = " + infraction.Id;
            ExecuteNonQuery(req);
        }
    }
}
