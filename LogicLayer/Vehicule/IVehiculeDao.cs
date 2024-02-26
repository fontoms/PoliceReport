namespace LogicLayer.Vehicule
{
    public interface IVehiculeDao
    {
        void Add(Vehicule vehicule);
        void Remove(Vehicule vehicule);
        void Update(Vehicule vehicule);
        List<Vehicule> GetAll();
        List<Vehicule> GetAllBySpecialisation(string specialisation);
        List<Vehicule> GetAllByNom(string nom);
        List<Vehicule> GetAllByNameContains(string name);
    }
}
