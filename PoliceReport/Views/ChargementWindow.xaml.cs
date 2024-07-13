using System.Windows;

namespace PoliceReport.Views
{
    /// <summary>
    /// Logique d'interaction pour ChargementWindow.xaml
    /// </summary>
    public partial class ChargementWindow : Window
    {
        #region Attributs
        private double _progressValue = 0;
        private double _maxValue = 100;
        #endregion

        #region Properties
        public double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                UpdateProgress(_progressValue);

                // Fermer la fenêtre si la valeur maximale est atteinte
                if (_progressValue >= _maxValue)
                {
                    Close();
                }
            }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                progressBar.Maximum = _maxValue;
            }
        }
        #endregion

        private static ChargementWindow? _instance;
        public static ChargementWindow Instance
        {
            get
            {
                _instance ??= new ChargementWindow();
                return _instance;
            }
        }

        private ChargementWindow(string nom = "")
        {
            InitializeComponent();
            Title = nom;
        }

        public void UpdateProgress(double value)
        {
            progressBar.Value = value;
        }
    }
}
