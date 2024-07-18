using LogicLayer.Action;
using System.Data.SQLite;

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
            string req = "INSERT INTO Actions (Nom, ActInfraction) VALUES ('" + action.Nom + "', '" + action.ActInfraction + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(LogicLayer.Action.Action action)
        {
            string req = "DELETE FROM Actions WHERE Id = " + action.Id;
            ExecuteNonQuery(req);
        }

        public void Update(LogicLayer.Action.Action action)
        {
            string req = "UPDATE Actions SET Nom = '" + action.Nom + "', ActInfraction = '" + action.ActInfraction + "' WHERE Id = " + action.Id;
            ExecuteNonQuery(req);
        }

        public List<LogicLayer.Action.Action> GetAllActions()
        {
            string req = "SELECT * FROM Actions ORDER BY Nom ASC";
            SQLiteDataReader reader = ExecuteReader(req);
            List<LogicLayer.Action.Action> actions = [];
            while (reader.Read())
            {
                actions.Add(new LogicLayer.Action.Action(reader.GetString(1), DateTime.Now));
            }
            reader.Close();
            CloseConnection();
            return actions;
        }

        public List<LogicLayer.Action.Action> GetAllByInfractions(int infraction)
        {
            string req = "SELECT * FROM Actions WHERE ActInfraction = '" + infraction + "' ORDER BY ActInfraction ASC";
            SQLiteDataReader reader = ExecuteReader(req);
            List<LogicLayer.Action.Action> actions = [];
            while (reader.Read())
            {
                actions.Add(new LogicLayer.Action.Action(reader.GetString(1), DateTime.Now));
            }
            reader.Close();
            CloseConnection();
            return actions;
        }

        public List<LogicLayer.Action.Action> GetAll()
        {
            string req = "SELECT * FROM Actions";
            SQLiteDataReader reader = ExecuteReader(req);
            List<LogicLayer.Action.Action> actions = [];
            while (reader.Read())
            {
                actions.Add(new LogicLayer.Action.Action(reader.GetInt16(0), reader.GetString(1), reader.GetInt16(2)));
            }
            reader.Close();
            CloseConnection();
            return actions;
        }
    }
}
