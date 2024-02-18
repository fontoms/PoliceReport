using LogicLayer.Infraction;

namespace StorageLayer.Dao
{
    public class InfractionsDao : BaseDao, IInfractionDao
    {
        public void Add(Infraction infraction)
        {
            throw new NotImplementedException();
        }

        public void Remove(Infraction infraction)
        {
            throw new NotImplementedException();
        }

        public List<Infraction> GetAll()
        {
            var req = "SELECT * FROM Infractions ORDER BY Type";
            var reader = ExecuteReader(req);
            var infractions = new List<Infraction>();
            while (reader.Read())
            {
                infractions.Add(new Infraction(reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            return infractions;
        }
    }
}
