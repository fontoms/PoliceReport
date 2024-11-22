using PoliceReport.Core.Action;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class ActionsDao : IActionDao
    {
        private readonly IDatabaseConnection _connection;

        public ActionsDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Core.Action.Action action)
        {
            string req = "INSERT INTO Actions (Nom, ActInfraction) VALUES ('" + action.Nom + "', '" + action.ActInfraction + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Core.Action.Action action)
        {
            string req = "DELETE FROM Actions WHERE Id = " + action.Id;
            _connection.ExecuteNonQuery(req);
        }

        public void Update(PoliceReport.Core.Action.Action action)
        {
            string req = "UPDATE Actions SET Nom = '" + action.Nom + "', ActInfraction = '" + action.ActInfraction + "' WHERE Id = " + action.Id;
            _connection.ExecuteNonQuery(req);
        }

        public List<PoliceReport.Core.Action.Action> GetAllActions()
        {
            string req = "SELECT * FROM Actions ORDER BY Nom ASC";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<PoliceReport.Core.Action.Action> actions = [];
            while (reader.Read())
            {
                actions.Add(new PoliceReport.Core.Action.Action(reader.GetString(1), DateTime.Now));
            }
            reader.Close();
            _connection.CloseConnection();
            return actions;
        }

        public List<PoliceReport.Core.Action.Action> GetAllByInfractions(int infraction)
        {
            string req = "SELECT * FROM Actions WHERE ActInfraction = '" + infraction + "' ORDER BY ActInfraction ASC";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<PoliceReport.Core.Action.Action> actions = [];
            while (reader.Read())
            {
                actions.Add(new PoliceReport.Core.Action.Action(reader.GetString(1), DateTime.Now));
            }
            reader.Close();
            _connection.CloseConnection();
            return actions;
        }

        public List<PoliceReport.Core.Action.Action> GetAll()
        {
            string req = "SELECT * FROM Actions";
            SQLiteDataReader reader = _connection.ExecuteReader(req);
            List<PoliceReport.Core.Action.Action> actions = [];
            while (reader.Read())
            {
                actions.Add(new PoliceReport.Core.Action.Action(reader.GetInt16(0), reader.GetString(1), reader.GetInt16(2)));
            }
            reader.Close();
            _connection.CloseConnection();
            return actions;
        }
    }
}
