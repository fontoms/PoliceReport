namespace LogicLayer.Unite
{
    public interface IUniteDao
    {
        void Add(Unite unite);
        void Remove(Unite unite);
        void Update(Unite unite);
        Unite GetType(string type);
        List<Unite> GetAll();
        List<Unite> GetAllBySpecialisation(string specialisation);
        List<Unite> GetAllByNom(string nom);
    }
}
