using LogicLayer.Outils;
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

        private void CheckForUpdates()
        {
            try
            {
                Version latestVersion, currentVersion;
                updateAvailable = CheckUpdateAvailable(out latestVersion, out currentVersion);

                if (updateAvailable)
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
                DisplayMessageBoxError(ex);
            }
        }

        private async void DisplayMessageBoxError(Exception ex)
        {
            MessageBoxResult result = MessageBox.Show($"Erreur lors de la vérification des mises à jour :\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            if (result == MessageBoxResult.OK)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                Close();
            }
        }

        private bool CheckUpdateAvailable(out Version latestVersion, out Version currentVersion)
        {
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            string releaseInfoJson = client.DownloadString(Constants.ApiRepoUrl);
            string latestVersionStr = releaseInfoJson.Split(new string[] { "\"tag_name\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim().Replace("\"", "");
            latestVersion = new Version(latestVersionStr);
            currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return latestVersion > currentVersion;
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
            try
            {
                // Désactiver le bouton pendant la mise à jour
                btnMiseAJour.IsEnabled = false;
                progressBar.Visibility = Visibility.Visible;

                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "request");
                string releaseInfoJson = await client.DownloadStringTaskAsync(Constants.ApiRepoUrl);
                string latestSetupUrl = releaseInfoJson.Split(new string[] { "\"browser_download_url\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim().Replace("\"", "").TrimEnd('}');
                string setupFileName = Path.Combine(Path.GetTempPath(), "PoliceReportSetup.exe");
                Uri uri = new Uri(latestSetupUrl);

                // Téléchargement de la mise à jour
                WebClient downloadClient = new WebClient();
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

                    // Redémarrage de l'application après la mise à jour (le fichier exe)
                    string restartPath = AppDomain.CurrentDomain.BaseDirectory + "PoliceReport.exe";
                    Process.Start(restartPath);

                    Application.Current.Shutdown();
                }
                else
                {
                    DisplayMessageBoxError(new Exception("Erreur lors du lancement du processus d'installation."));
                }
            }
            catch (Exception ex)
            {
                DisplayMessageBoxError(ex);
            }
            finally
            {
                // Réactiver le bouton une fois la mise à jour terminée ou en cas d'erreur
                btnMiseAJour.IsEnabled = true;
            }
        }

        private async Task WaitForMsiexecProcesses()
        {
            const string processName = "msiexec";

            while (true)
            {
                try
                {
                    // Rechercher tous les processus avec ce nom
                    Process[] processes = Process.GetProcessesByName(processName);

                    bool allProcessesExited = true;

                    foreach (Process process in processes)
                    {
                        try
                        {
                            // Vérifier si le processus est encore en cours d'exécution
                            if (!process.HasExited)
                            {
                                allProcessesExited = false; // Il y a au moins un processus qui n'a pas encore terminé
                            }
                        }
                        catch (System.ComponentModel.Win32Exception)
                        {
                        }
                    }

                    if (allProcessesExited)
                    {
                        break; // Sortir de la boucle si tous les processus ont terminé
                    }
                }
                catch (Exception ex)
                {
                    // Gérer toute autre exception
                    DisplayMessageBoxError(ex);
                }

                // Attendre 1 seconde avant de vérifier à nouveau
                await Task.Delay(1000);
            }
        }
    }
}
