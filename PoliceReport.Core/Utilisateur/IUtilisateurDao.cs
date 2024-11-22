namespace PoliceReport.Core.Utilisateur
{
    public interface IUtilisateurDao
    {
        void Add(Utilisateur user);
        void Remove(Utilisateur user);
        void Update(Utilisateur user);
        List<Utilisateur> GetAll();
    }
}
