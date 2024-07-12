using LogicLayer.Action;

namespace StorageLayer.Dao
{
    public class ActionsDao : BaseDao, IActionDao
    {
        private static ActionsDao? _instance;

        private ActionsDao() : base() { }

        public static ActionsDao Instance
        {
            get
            {
                _instance ??= new ActionsDao();
                return _instance;
            }
        }

        public void Add(LogicLayer.Action.Action action)
        {
            var req = "INSERT INTO Actions (Nom, ActInfraction) VALUES ('" + action.Nom + "', '" + action.ActInfraction + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(LogicLayer.Action.Action action)
        {
            var req = "DELETE FROM Actions WHERE Id = " + action.Id;
            ExecuteNonQuery(req);
        }

        public void Update(LogicLayer.Action.Action action)
        {
            var req = "UPDATE Actions SET Nom = '" + action.Nom + "', ActInfraction = '" + action.ActInfraction + "' WHERE Id = " + action.Id;
            ExecuteNonQuery(req);
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
            CloseConnection();
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
            CloseConnection();
            return actions;
        }

        public List<object> GetAll()
        {
            var req = "SELECT * FROM Actions";
            var reader = ExecuteReader(req);
            var actions = new List<object>();
            while (reader.Read())
            {
                actions.Add(new LogicLayer.Action.Action(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
            reader.Close();
            CloseConnection();
            return actions;
        }
    }
}
