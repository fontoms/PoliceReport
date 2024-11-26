namespace PoliceReport.Core.Infraction
{
    public interface IInfractionDao
    {
        void Add(Infraction infraction);
        void Remove(Infraction infraction);
        void Update(Infraction infraction);
        List<Infraction> GetAll();
    }
}
