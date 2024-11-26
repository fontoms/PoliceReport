using System.ComponentModel;

namespace PoliceReport.Core.Effectif
{
    public class Effectif : INotifyPropertyChanged
    {
        #region Attributes
        private int _id;
        private string _idDiscord;
        private string _nom;
        private string _prenom;
        private int _effGrade;
        private Grade.Grade _grade;
        private string _positionVehicule;
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

        public string IdDiscord
        {
            get => _idDiscord;
            set
            {
                if (_idDiscord != value)
                {
                    _idDiscord = value;
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

        public int EffGrade
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

        public Effectif(int id, string idDiscord, string nom, string prenom, int effGrade)
        {
            Id = id;
            IdDiscord = idDiscord;
            Nom = nom;
            Prenom = prenom;
            EffGrade = effGrade;
        }

        public Effectif(int id, string idDiscord, string nom, string prenom, int effGrade, Grade.Grade grade, string positionVehicule)
        {
            Id = id;
            IdDiscord = idDiscord;
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
