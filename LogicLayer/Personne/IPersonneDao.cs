namespace LogicLayer.Personne
{
    public interface IPersonneDao
    {
        void Add(Personne personne);
        void Remove(Personne personne);
        List<Personne> GetAll();
    }
}
