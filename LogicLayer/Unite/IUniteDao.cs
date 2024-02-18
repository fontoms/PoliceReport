namespace LogicLayer.Unite
{
    public interface IUniteDao
    {
        void Add(Unite unite);
        void Remove(Unite unite);
        List<Unite> GetAll();
        List<Unite> GetAllBySpecialisation(string specialisation);
        List<Unite> GetAllByNom(string nom);
    }
}
