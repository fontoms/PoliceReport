namespace PoliceReport.Core.Effectif
{
    public interface IEffectifDao
    {
        void Add(Effectif effectif);
        void Remove(Effectif effectif);
        void Update(Effectif effectif);
        List<Effectif> GetAll();
        List<Effectif> GetAllByGrade(int grade);
        List<Effectif> GetAllEffectifs();
    }
}
