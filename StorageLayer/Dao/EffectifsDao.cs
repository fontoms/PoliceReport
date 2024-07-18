using LogicLayer.Effectif;
using System.Data.SQLite;

namespace StorageLayer.Dao
{
    public class EffectifsDao : BaseDao, IEffectifDao
    {
        private static EffectifsDao? _instance;

        private EffectifsDao() : base() { }

        public static EffectifsDao Instance
        {
            get
            {
                _instance ??= new EffectifsDao();
                return _instance;
            }
        }

        public void Add(Effectif effectif)
        {
            string req = "INSERT INTO Effectifs (IdDiscord, Nom, Prenom, EffGrade) VALUES ('" + effectif.IdDiscord + "', '" + effectif.Nom + "', '" + effectif.Prenom + "', '" + effectif.EffGrade + "')";
            ExecuteNonQuery(req);
        }

        public void Remove(Effectif effectif)
        {
            string req = "DELETE FROM Effectifs WHERE Id = " + effectif.Id;
            ExecuteNonQuery(req);
        }

        public void Update(Effectif effectif)
        {
            string req = "UPDATE Effectifs SET IdDiscord = '" + effectif.IdDiscord + "', Nom = '" + effectif.Nom + "', Prenom = '" + effectif.Prenom + "', EffGrade = '" + effectif.EffGrade + "' WHERE Id = " + effectif.Id;
            ExecuteNonQuery(req);
        }

        public List<Effectif> GetAll()
        {
            string req = "SELECT * FROM Effectifs";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Effectif> effectifs = [];
            while (reader.Read())
            {
                effectifs.Add(new Effectif(reader.GetInt16(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt16(4)));
            }
            reader.Close();
            CloseConnection();
            return effectifs;
        }

        public List<Effectif> GetAllByGrade(int grade)
        {
            string req = "SELECT * FROM Effectifs WHERE EffGrade = '" + grade + "' INNER JOIN Grades ON EffGrade = Type ORDER BY Grades.Id ASC";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Effectif> effectifs = [];
            while (reader.Read())
            {
                effectifs.Add(new Effectif(reader.GetInt16(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt16(4)));
            }
            reader.Close();
            CloseConnection();
            return effectifs;
        }

        public List<Effectif> GetAllEffectifs()
        {
            string req = "SELECT * FROM Effectifs ORDER BY Nom ASC";
            SQLiteDataReader reader = ExecuteReader(req);
            List<Effectif> effectifs = [];
            while (reader.Read())
            {
                effectifs.Add(new Effectif(reader.GetInt16(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt16(4)));
            }
            reader.Close();
            CloseConnection();
            return effectifs;
        }
    }
}
