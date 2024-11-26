namespace PoliceReport.Core.Vehicule
{
    public class Vehicule
    {
        #region Attributs
        private int _id;
        private int _vehSpecialisation;
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

        public int VehSpecialisation
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

        public Vehicule(int vehSpecialisation, string nom)
        {
            VehSpecialisation = vehSpecialisation;
            Nom = nom;
        }

        public Vehicule(int id, int vehSpecialisation, string nom)
        {
            Id = id;
            VehSpecialisation = vehSpecialisation;
            Nom = nom;
        }
    }
}
