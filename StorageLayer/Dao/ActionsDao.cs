using LogicLayer.Action;

namespace StorageLayer.Dao
{
    public class ActionsDao : BaseDao, IActionDao
    {
        public ActionsDao() : base() { }

        public void Add(LogicLayer.Action.Action action)
        {
            throw new NotImplementedException();
        }

        public void Remove(LogicLayer.Action.Action action)
        {
            throw new NotImplementedException();
        }

        public List<LogicLayer.Action.Action> GetAllActions()
        {
            var req = "SELECT * FROM Actions ORDER BY Nom ASC";
            var reader = ExecuteReader(req);
            var actions = new List<LogicLayer.Action.Action>();
            while (reader.Read())
            {
                actions.Add(new LogicLayer.Action.Action(reader.GetString(1), DateTime.Now));
            }
            reader.Close();
            return actions;
        }

        public List<LogicLayer.Action.Action> GetAllByInfractions(string infraction)
        {
            var req = "SELECT * FROM Actions WHERE ActInfraction = '" + infraction + "' ORDER BY ActInfraction ASC";
            var reader = ExecuteReader(req);
            var actions = new List<LogicLayer.Action.Action>();
            while (reader.Read())
            {
                actions.Add(new LogicLayer.Action.Action(reader.GetString(1), DateTime.Now));
            }
            reader.Close();
            return actions;
        }
    }
}
