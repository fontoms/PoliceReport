namespace LogicLayer.Effectif
{
    public interface IEffectifDao
    {
        void Add(Effectif effectif);
        void Remove(Effectif effectif);
        void Update(Effectif effectif);
        List<Effectif> GetAll();
        List<Effectif> GetAllEffectifs();
        List<Effectif> GetAllByGrade(string grade);
    }
}
