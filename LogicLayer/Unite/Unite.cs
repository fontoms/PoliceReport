namespace LogicLayer.Unite
{
    public class Unite
    {
        #region Attributs
        private int _id;
        private string _type;
        private string _nom;
        private string _unitSpe;
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

        public string UnitSpe
        {
            get => _unitSpe;
            set
            {
                if (_unitSpe != value)
                {
                    _unitSpe = value;
                }
            }
        }
        #endregion

        public Unite()
        {
        }

        public Unite(string type, string nom, string uniSpe)
        {
            Type = type;
            Nom = nom;
            UnitSpe = uniSpe;
        }

        public Unite(int id, string type, string nom, string uniSpe)
        {
            Id = id;
            Type = type;
            Nom = nom;
            UnitSpe = uniSpe;
        }
    }
}
