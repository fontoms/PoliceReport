namespace LogicLayer.Specialisation
{
    public class Specialisation
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

        public Specialisation()
        {
        }

        public Specialisation(string nom)
        {
            Nom = nom;
        }

        public Specialisation(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }
    }
}
