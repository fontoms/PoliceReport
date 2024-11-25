using PoliceReport.Core.Tools;
using PoliceReport.Core.Tools.Updates;
using PoliceReport.Updates;
using System.Reflection;
using System.Windows;

namespace PoliceReport.Views
{
    /// <summary>
    /// Logique d'interaction pour MiseAJourWindow.xaml
    /// </summary>
    public partial class MiseAJourWindow : Window
    {
        private VersionInfo updateAvailable = null;

        public MiseAJourWindow()
        {
            InitializeComponent();
            CheckForInternetConnection();
        }

        private void CheckForInternetConnection()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Exception ex = new Exception("Aucune connexion Internet n'est disponible.");
                DisplayMessageBoxError(ex);
            }
            else
            {
                CheckForUpdates();
            }
        }

        private async void CheckForUpdates()
        {
            try
            {
                updateAvailable = await Updater.CheckUpdateAvailableAsync();

                if (updateAvailable.LatestVersion == null)
                {
                    // Afficher un avertissement si la vérification des mises à jour a échoué
                    Task.Run(() => MessageBox.Show("La vérification des mises à jour a échoué. Veuillez réessayer plus tard.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning));
                }

                if (updateAvailable.LatestVersion > updateAvailable.CurrentVersion)
                {
                    // Si une mise à jour est disponible, afficher le bouton de mise à jour
                    btnMiseAJour.Content = "Mettre à jour";
                    nouvelleMajLbl.Content = "Une nouvelle mise à jour est disponible !";
                    progressBar.Visibility = Visibility.Hidden;
                    versionLbl.Content = $"Version actuelle : {updateAvailable.CurrentVersion} - Dernière version : {updateAvailable.LatestVersion}";
                }
                else
                {
                    // Si aucune mise à jour n'est disponible, changer le texte du bouton et afficher MainWindow
                    btnMiseAJour.Content = "Lancer";
                    nouvelleMajLbl.Content = $"Profitez bien de {Assembly.GetExecutingAssembly().GetName().Name} !";
                    progressBar.Visibility = Visibility.Hidden;
                    versionLbl.Content = $"Version actuelle : {updateAvailable.CurrentVersion}";
                }
            }
            catch (Exception ex)
            {
                DisplayMessageBoxError(ex);
            }
        }

        private void DisplayMessageBoxError(Exception ex)
        {
            MessageBoxResult result = MessageBox.Show($"Erreur lors de la vérification des mises à jour :\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            if (result == MessageBoxResult.OK)
            {
                MainWindow mainWindow = new MainWindow(Startup.ConfigureServices());
                mainWindow.Show();
                Close();
            }
            else
            {
                Close();
            }
        }

        private async void btnMiseAJour_Click(object sender, RoutedEventArgs e)
        {
            if (updateAvailable != null && updateAvailable.LatestVersion > updateAvailable.CurrentVersion)
            {
                GitHubVersionProvider versionProvider = new GitHubVersionProvider(Constants.ApiRepoUrl);
                WpfUpdateUserInterface uiManager = new WpfUpdateUserInterface(progressBar, btnMiseAJour);
                ApplicationUpdater updater = new ApplicationUpdater(versionProvider, uiManager);
                UpdateManager updateManager = new UpdateManager(updater);

                await updateManager.PerformUpdateAsync();
            }
            else
            {
                MainWindow mainWindow = new MainWindow(Startup.ConfigureServices());
                mainWindow.Show();
                Close();
            }
        }
    }
}
