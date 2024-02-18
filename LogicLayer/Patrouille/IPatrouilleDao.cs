namespace LogicLayer.Patrouille
{
    public interface IPatrouilleDao
    {
        void Add(Patrouille patrouille);
        void Remove(Patrouille patrouille);
        List<Patrouille> GetAll();
    }
}
