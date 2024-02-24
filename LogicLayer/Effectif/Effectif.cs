using System.ComponentModel;

namespace LogicLayer.Effectif
{
    public class Effectif : INotifyPropertyChanged
    {
        #region Attributes
        private string _id;
        private string _nom;
        private string _prenom;
        private string _effGrade;
        private Grade.Grade _grade;
        private string _positionVehicule;
        #endregion

        #region Properties
        public string Id
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

        public string Prenom
        {
            get => _prenom;
            set
            {
                if (_prenom != value)
                {
                    _prenom = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EffGrade
        {
            get => _effGrade;
            set
            {
                if (_effGrade != value)
                {
                    _effGrade = value;
                    OnPropertyChanged();
                }
            }
        }

        public Grade.Grade Grade
        {
            get => _grade;
            set
            {
                if (_grade != value)
                {
                    _grade = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PositionVehicule
        {
            get => _positionVehicule;
            set
            {
                if (_positionVehicule != value)
                {
                    _positionVehicule = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public Effectif()
        {
        }

        public Effectif(string id, string nom, string prenom, string effGrade)
        {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            EffGrade = effGrade;
        }

        public Effectif(string id, string nom, string prenom, string effGrade, Grade.Grade? grade, string? positionVehicule)
        {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            EffGrade = effGrade;
            Grade = grade;
            PositionVehicule = positionVehicule;
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
