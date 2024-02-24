using LogicLayer.Effectif;

namespace StorageLayer.Dao
{
    public class EffectifsDao : BaseDao, IEffectifDao
    {
        public EffectifsDao() : base() { }

        public void Add(Effectif effectif)
        {
            throw new NotImplementedException();
        }

        public List<Effectif> GetAllByGrade(string grade)
        {
            var req = "SELECT * FROM Effectifs WHERE EffGrade = '" + grade + "' INNER JOIN Grades ON EffGrade = Type ORDER BY Grades.Id ASC";
            var reader = ExecuteReader(req);
            var effectifs = new List<Effectif>();
            while (reader.Read())
            {
                effectifs.Add(new Effectif(Convert.ToString(reader.GetValue(0)), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            reader.Close();
            return effectifs;
        }

        public List<Effectif> GetAllEffectifs()
        {
            var req = "SELECT * FROM Effectifs ORDER BY Nom ASC";
            var reader = ExecuteReader(req);
            var effectifs = new List<Effectif>();
            while (reader.Read())
            {
                effectifs.Add(new Effectif(Convert.ToString(reader.GetValue(0)), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            reader.Close();
            return effectifs;
        }

        public void Remove(Effectif effectif)
        {
            throw new NotImplementedException();
        }
    }
}
