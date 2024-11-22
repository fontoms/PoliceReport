namespace PoliceReport.Core.Grade
{
    public class Grade
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

        public Grade()
        {
        }

        public Grade(string nom)
        {
            Nom = nom;
        }

        public Grade(int id, string nom)
        {
            Id = id;
            Nom = nom;
        }
    }
}
