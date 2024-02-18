namespace LogicLayer.Infraction
{
    public interface IInfractionDao
    {
        void Add(Infraction infraction);
        void Remove(Infraction infraction);
        List<Infraction> GetAll();
    }
}
