namespace LogicLayer.Action
{
    public interface IActionDao
    {
        void Add(Action action);
        void Remove(Action action);
        List<Action> GetAllActions();
        List<Action> GetAllByInfractions(string infraction);
    }
}
