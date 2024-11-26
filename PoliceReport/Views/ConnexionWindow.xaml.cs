using Microsoft.Extensions.DependencyInjection;
using PoliceReport.Core.Tools;
using PoliceReport.Core.Tools.Encryption;
using PoliceReport.Core.Utilisateur;
using System.Windows;

namespace PoliceReport.Views
{
    /// <summary>
    /// Logique d'interaction pour ConnexionWindow.xaml
    /// </summary>
    public partial class ConnexionWindow : Window
    {
        public bool MotDePasseCorrect { get; private set; }
        public Utilisateur User { get; private set; }

        private readonly IUtilisateurDao _utilisateurDao;

        public ConnexionWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _utilisateurDao = serviceProvider.GetRequiredService<IUtilisateurDao>();
            MotDePasseCorrect = false;
            userBox.Focus();
        }

        private void connectionBtn_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Utilisateur user = Constants.Users.FirstOrDefault(u => u.Username == userBox.Text && u.Password == HashHelper.CalculateSHA256(passBox.Password));
#else
            Utilisateur user = _utilisateurDao.GetUser(userBox.Text, HashHelper.CalculateSHA256(passBox.Password));
#endif
            if (user != null)
            {
                MotDePasseCorrect = true;
                User = user;
                Close();
            }
            else
            {
                MessageBox.Show("Mot de passe incorrect", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
