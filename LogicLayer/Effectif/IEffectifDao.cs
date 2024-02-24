namespace LogicLayer.Effectif
{
    public interface IEffectifDao
    {
        void Add(Effectif effectif);
        void Remove(Effectif effectif);
        List<Effectif> GetAllEffectifs();
        List<Effectif> GetAllByGrade(string grade);
    }
}
