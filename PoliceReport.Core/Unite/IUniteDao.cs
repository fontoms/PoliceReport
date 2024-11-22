namespace PoliceReport.Core.Unite
{
    public interface IUniteDao
    {
        void Add(Unite unite);
        void Remove(Unite unite);
        void Update(Unite unite);
        Unite GetId(int id);
        Unite GetType(string type);
        List<Unite> GetAll();
        List<Unite> GetAllBySpecialisation(int specialisation);
        List<Unite> GetAllByNom(string nom);
    }
}
