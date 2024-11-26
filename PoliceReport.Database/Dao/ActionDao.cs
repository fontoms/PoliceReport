using PoliceReport.Core.Action;
using System.Data.SQLite;

namespace PoliceReport.Database.Dao
{
    public class ActionDao : IActionDao
    {
        private readonly IDatabaseConnection _connection;

        public ActionDao(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public void Add(Core.Action.Action action)
        {
            string req = "INSERT INTO Action (Nom, ActInfraction) VALUES ('" + action.Nom + "', '" + action.ActInfraction + "')";
            _connection.ExecuteNonQuery(req);
        }

        public void Remove(Core.Action.Action action)
        {
            string req = "DELETE FROM Action WHERE Id = " + action.Id;
            _connection.ExecuteNonQuery(req);
        }

        public void Update(PoliceReport.Core.Action.Action action)
        {
            string req = "UPDATE Action SET Nom = '" + action.Nom + "', ActInfraction = '" + action.ActInfraction + "' WHERE Id = " + action.Id;
            _connection.ExecuteNonQuery(req);
        }

        public List<PoliceReport.Core.Action.Action> GetAllActions()
        {
            string req = "SELECT * FROM Action ORDER BY Nom ASC";
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
            string req = "SELECT * FROM Action WHERE ActInfraction = '" + infraction + "' ORDER BY ActInfraction ASC";
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
            string req = "SELECT * FROM Action";
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
