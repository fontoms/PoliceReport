namespace LogicLayer.Infraction
{
    public class Infraction
    {
        #region Attributs
        private int _id;
        private string _nom;
        #endregion

        #region Properties
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                }
            }
        }

        public string Nom
        {
            get => _nom;
            set
            {
                if (_nom != value)
                {
                    _nom = value;
                }
            }
        }
        #endregion

        public Infraction()
        {
        }

        public Infraction(string nom)
        {
            Nom = nom;
        }

        public Infraction(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }
    }
}
