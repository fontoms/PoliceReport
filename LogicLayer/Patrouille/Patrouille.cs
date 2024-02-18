using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LogicLayer.Patrouille
{
    public class Patrouille : INotifyPropertyChanged
    {
        #region Attributes
        private int _id;
        private Unite.Unite _indicatif;
        private Vehicule.Vehicule _vehicule;
        private List<Personne.Personne> _effectifs;
        private DateTime _heureDebutPatrouille;
        private DateTime? _heureFinPatrouille;
        #endregion

        #region Properties
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public Unite.Unite Indicatif
        {
            get { return _indicatif; }
            set
            {
                if (_indicatif != value)
                {
                    _indicatif = value;
                    OnPropertyChanged();
                }
            }
        }

        public Vehicule.Vehicule Vehicule
        {
            get { return _vehicule; }
            set
            {
                if (_vehicule != value)
                {
                    _vehicule = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Personne.Personne> Effectifs
        {
            get { return _effectifs; }
            set
            {
                if (_effectifs != value)
                {
                    _effectifs = value;
                    OnPropertyChanged();
                }
            }
        }

        public System.DateTime HeureDebutPatrouille
        {
            get { return _heureDebutPatrouille; }
            set
            {
                if (_heureDebutPatrouille != value)
                {
                    _heureDebutPatrouille = value;
                    OnPropertyChanged();
                }
            }
        }

        public System.DateTime? HeureFinPatrouille
        {
            get { return _heureFinPatrouille; }
            set
            {
                if (_heureFinPatrouille != value)
                {
                    _heureFinPatrouille = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public Patrouille()
        {
            HeureDebutPatrouille = DateTime.Now;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

