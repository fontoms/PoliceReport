namespace PoliceReport.Core.Vehicule
{
    public interface IVehiculeDao
    {
        void Add(Vehicule vehicule);
        void Remove(Vehicule vehicule);
        void Update(Vehicule vehicule);
        List<Vehicule> GetAll();
        List<Vehicule> GetAllBySpecialisation(int specialisation);
        List<Vehicule> GetAllByNom(string nom);
        List<Vehicule> GetAllByNameContains(string name);
    }
}
