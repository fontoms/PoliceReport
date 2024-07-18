using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LogicLayer.Action
{
    public class Action : INotifyPropertyChanged
    {
        #region Attributes
        private int _id;
        private string _nom;
        private DateTime _heure;
        private int _actInfraction;
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Heure
        {
            get => _heure;
            set
            {
                if (_heure != value)
                {
                    _heure = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ActInfraction
        {
            get => _actInfraction;
            set
            {
                if (_actInfraction != value)
                {
                    _actInfraction = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public Action() { }

        public Action(string nom, DateTime heure)
        {
            Nom = nom;
            Heure = heure;
        }

        public Action(int id, string nom, int actInfraction)
        {
            Id = id;
            Nom = nom;
            ActInfraction = actInfraction;
        }

        public Action(int id, string nom, DateTime heure, int actInfraction)
        {
            Id = id;
            Nom = nom;
            Heure = heure;
            ActInfraction = actInfraction;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
