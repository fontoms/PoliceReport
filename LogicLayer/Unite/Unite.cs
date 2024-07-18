namespace LogicLayer.Unite
{
    public class Unite
    {
        #region Attributs
        private int _id;
        private string _nom;
        private int _unitSpecialisation;
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

        public int UnitSpecialisation
        {
            get => _unitSpecialisation;
            set
            {
                if (_unitSpecialisation != value)
                {
                    _unitSpecialisation = value;
                }
            }
        }
        #endregion

        public Unite()
        {
        }

        public Unite(string nom, int uniSpecialisation)
        {
            Nom = nom;
            UnitSpecialisation = uniSpecialisation;
        }

        public Unite(int id, string nom, int uniSpecialisation)
        {
            Id = id;
            Nom = nom;
            UnitSpecialisation = uniSpecialisation;
        }
    }
}
