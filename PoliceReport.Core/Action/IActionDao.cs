namespace PoliceReport.Core.Action
{
    public interface IActionDao
    {
        void Add(Action action);
        void Remove(Action action);
        void Update(Action action);
        List<Action> GetAll();
        List<Action> GetAllActions();
        List<Action> GetAllByInfractions(int infraction);
    }
}
