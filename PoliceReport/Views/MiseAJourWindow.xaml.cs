using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace PoliceReport.Views
{
    /// <summary>
    /// Logique d'interaction pour MiseAJourWindow.xaml
    /// </summary>
    public partial class MiseAJourWindow : Window
    {
        private bool updateAvailable = false;

        public MiseAJourWindow()
        {
            InitializeComponent();
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            string owner = "Fontom71";
            string repo = "PoliceReport";
            string repoUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "request");
                string releaseInfoJson = client.DownloadString(repoUrl);
                string latestVersionStr = releaseInfoJson.Split(new string[] { "\"tag_name\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim().Replace("\"", "");
                Version latestVersion = new Version(latestVersionStr);
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                if (latestVersion > currentVersion)
                {
                    // Si une mise à jour est disponible, afficher le bouton de mise à jour
                    btnMiseAJour.Content = "Mettre à jour";
                    nouvelleMajLbl.Content = "Une nouvelle mise à jour est disponible !";
                    progressBar.Visibility = Visibility.Hidden;
                    versionLbl.Content = $"Version actuelle : {currentVersion} - Dernière version : {latestVersion}";
                    updateAvailable = true;
                }
                else
                {
                    // Si aucune mise à jour n'est disponible, changer le texte du bouton et afficher MainWindow
                    btnMiseAJour.Content = "Lancer";
                    nouvelleMajLbl.Content = $"Profitez bien de {Assembly.GetExecutingAssembly().GetName().Name} !";
                    progressBar.Visibility = Visibility.Hidden;
                    versionLbl.Content = $"Version actuelle : {currentVersion}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification des mises à jour : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnMiseAJour_Click(object sender, RoutedEventArgs e)
        {
            if (updateAvailable)
            {
                await UpdateApplication();
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }

        private async Task UpdateApplication()
        {
            string owner = "Fontom71";
            string repo = "PoliceReport";
            string repoUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

            try
            {
                // Désactiver le bouton pendant la mise à jour
                btnMiseAJour.IsEnabled = false;
                progressBar.Visibility = Visibility.Visible;

                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "request");
                string releaseInfoJson = await client.DownloadStringTaskAsync(repoUrl);
                string latestSetupUrl = releaseInfoJson.Split(new string[] { "\"browser_download_url\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim().Replace("\"", "").TrimEnd('}');
                string setupFileName = Path.Combine(Path.GetTempPath(), "PoliceReportSetup.exe");

                // Téléchargement de la mise à jour
                WebClient downloadClient = new WebClient();
                Uri uri = new Uri(latestSetupUrl);
                downloadClient.DownloadProgressChanged += (s, args) =>
                {
                    Dispatcher.Invoke(() => progressBar.Value = args.ProgressPercentage);
                };

                await downloadClient.DownloadFileTaskAsync(uri, setupFileName);

                progressBar.Visibility = Visibility.Hidden;

                // Lancement du processus d'installation avec élévation de privilèges
                ProcessStartInfo setupStartInfo = new ProcessStartInfo
                {
                    FileName = setupFileName,
                    UseShellExecute = true,
                    Verb = "runas" // Exécute le processus avec élévation de privilèges
                };

                Process setupProcess = Process.Start(setupStartInfo);
                if (setupProcess != null)
                {
                    // Attendre que le processus d'installation soit terminé
                    setupProcess.WaitForExit();

                    // Attendre la fin de tous les processus msiexec associés à l'installation
                    await WaitForMsiexecProcesses();

                    // Redémarrage de l'application après la mise à jour
                    Process.Start(Assembly.GetExecutingAssembly().Location);
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show("Impossible de lancer le processus d'installation.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Réactiver le bouton une fois la mise à jour terminée ou en cas d'erreur
                btnMiseAJour.IsEnabled = true;
            }
        }

        private async Task WaitForMsiexecProcesses()
        {
            const string msiexecName = "msiexec";

            // Attendre jusqu'à ce qu'il n'y ait plus de processus msiexec en cours d'exécution
            while (Process.GetProcessesByName(msiexecName).Any())
            {
                await Task.Delay(1000); // Attendre 1 seconde avant de vérifier à nouveau
            }
        }
    }
}
