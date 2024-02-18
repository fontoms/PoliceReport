using LogicLayer.Specialisation;

namespace StorageLayer.Dao
{
    public class SpecialisationsDao : BaseDao, ISpecialisationDao
    {
        public SpecialisationsDao() : base() { }

        public void Add(Specialisation specialisation)
        {
            throw new NotImplementedException();
        }

        public void Remove(Specialisation specialisation)
        {
            throw new NotImplementedException();
        }

        public List<Specialisation> GetAll()
        {
            var req = "SELECT * FROM Specialisations";
            var reader = ExecuteReader(req);
            var specialisations = new List<Specialisation>();
            while (reader.Read())
            {
                specialisations.Add(new Specialisation(reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            return specialisations;
        }
    }
}
