namespace LogicLayer.Grade
{
    public class Grade
    {
        #region Attributs
        private int _id;
        private string _type;
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

        public string Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
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

        public Grade(string type, string nom)
        {
            Type = type;
            Nom = nom;
        }

        public Grade(int id, string type, string nom)
        {
            Id = id;
            Type = type;
            Nom = nom;
        }
    }
}
