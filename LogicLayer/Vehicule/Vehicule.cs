namespace LogicLayer.Vehicule
{
    public class Vehicule
    {
        #region Attributs
        private int _id;
        private string _vehSpecialisation;
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

        public string VehSpecialisation
        {
            get => _vehSpecialisation;
            set
            {
                if (_vehSpecialisation != value)
                {
                    _vehSpecialisation = value;
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

        public Vehicule()
        {
        }

        public Vehicule(string vehSpecialisation, string nom)
        {
            VehSpecialisation = vehSpecialisation;
            Nom = nom;
        }

        public Vehicule(int id, string vehSpecialisation, string nom)
        {
            Id = id;
            VehSpecialisation = vehSpecialisation;
            Nom = nom;
        }
    }
}
