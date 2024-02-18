using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LogicLayer.Personne
{
    public class Personne : INotifyPropertyChanged
    {
        #region Attributes
        private string _id;
        private Grade.Grade _grade;
        private string _positionVehicule;
        #endregion

        #region Properties
        public string Id
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

        public Grade.Grade Grade
        {
            get { return _grade; }
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
            get { return _positionVehicule; }
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

        public event PropertyChangedEventHandler? PropertyChanged;

        public Personne()
        {
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
