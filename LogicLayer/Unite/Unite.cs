namespace LogicLayer.Unite
{
    public class Unite
    {
        #region Attributs
        private int _id;
        private string _type;
        private string _nom;
        private string _uniSpe;
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

        public string UniSpe
        {
            get => _uniSpe;
            set
            {
                if (_uniSpe != value)
                {
                    _uniSpe = value;
                }
            }
        }
        #endregion

        public Unite(string type, string nom, string uniSpe)
        {
            Type = type;
            Nom = nom;
            UniSpe = uniSpe;
        }
    }
}
