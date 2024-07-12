using LogicLayer.Outils;
using LogicLayer.Outils.Cryptage;
using System.Windows;

namespace PoliceReport.Views
{
    /// <summary>
    /// Logique d'interaction pour ConnexionWindow.xaml
    /// </summary>
    public partial class ConnexionWindow : Window
    {
        private const string MotDePasseAttendu = Constants.AdminPassword;

        public bool MotDePasseCorrect { get; private set; }

        public ConnexionWindow()
        {
            InitializeComponent();
            MotDePasseCorrect = false;
        }

        private void connectionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MotDePasseAttendu == HashHelper.CalculateSHA256(passBox.Password))
            {
                MotDePasseCorrect = true;
                Close();
            }
            else
            {
                MessageBox.Show("Mot de passe incorrect", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
