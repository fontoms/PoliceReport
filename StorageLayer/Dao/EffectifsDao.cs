using LogicLayer.Effectif;

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
            var req = "INSERT INTO Effectifs (IdDiscord, Nom, Prenom, EffGrade) VALUES ('" + effectif.IdDiscord + "', '" + effectif.Nom + "', '" + effectif.Prenom + "', '" + effectif.EffGrade + "')";
            ExecuteNonQuery(req);
        }

        public List<Effectif> GetAll()
        {
            var req = "SELECT * FROM Effectifs";
            var reader = ExecuteReader(req);
            var effectifs = new List<Effectif>();
            while (reader.Read())
            {
                effectifs.Add(new Effectif(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
            }
            reader.Close();
            CloseConnection();
            return effectifs;
        }

        public List<Effectif> GetAllByGrade(string grade)
        {
            var req = "SELECT * FROM Effectifs WHERE EffGrade = '" + grade + "' INNER JOIN Grades ON EffGrade = Type ORDER BY Grades.Id ASC";
            var reader = ExecuteReader(req);
            var effectifs = new List<Effectif>();
            while (reader.Read())
            {
                effectifs.Add(new Effectif(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
            }
            reader.Close();
            CloseConnection();
            return effectifs;
        }

        public List<Effectif> GetAllEffectifs()
        {
            var req = "SELECT * FROM Effectifs ORDER BY Nom ASC";
            var reader = ExecuteReader(req);
            var effectifs = new List<Effectif>();
            while (reader.Read())
            {
                effectifs.Add(new Effectif(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
            }
            reader.Close();
            CloseConnection();
            return effectifs;
        }

        public void Remove(Effectif effectif)
        {
            var req = "DELETE FROM Effectifs WHERE Id = " + effectif.Id;
            ExecuteNonQuery(req);
        }

        public void Update(Effectif effectif)
        {
            var req = "UPDATE Effectifs SET IdDiscord = '" + effectif.IdDiscord + "', Nom = '" + effectif.Nom + "', Prenom = '" + effectif.Prenom + "', EffGrade = '" + effectif.EffGrade + "' WHERE Id = " + effectif.Id;
            ExecuteNonQuery(req);
        }
    }
}
