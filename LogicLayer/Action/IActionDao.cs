namespace LogicLayer.Action
{
    public interface IActionDao
    {
        void Add(Action action);
        void Remove(Action action);
        void Update(Action action);
        List<object> GetAll();
        List<Action> GetAllActions();
        List<Action> GetAllByInfractions(string infraction);
    }
}
