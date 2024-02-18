namespace LogicLayer.Specialisation
{
    public interface ISpecialisationDao
    {
        void Add(Specialisation specialisation);
        void Remove(Specialisation specialisation);
        List<Specialisation> GetAll();
    }
}
