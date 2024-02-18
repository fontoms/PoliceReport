namespace LogicLayer.Vehicule
{
    public class Vehicule
    {
        #region Attributs
        private int _id;
        private string _vehSpe;
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

        public string VehSpe
        {
            get => _vehSpe;
            set
            {
                if (_vehSpe != value)
                {
                    _vehSpe = value;
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

        public Vehicule(string vehSpe, string nom)
        {
            VehSpe = vehSpe;
            Nom = nom;
        }
    }
}
