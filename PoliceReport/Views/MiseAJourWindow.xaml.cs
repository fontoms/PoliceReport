using PoliceReport.Core.Outils;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
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
                updateAvailable = Updater.CheckUpdateAvailable(out latestVersion, out currentVersion);

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
            if (updateAvailable)
            {
                await UpdateApplication();
            }
            else
            {
                MainWindow mainWindow = new MainWindow(Startup.ConfigureServices());
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

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "request");
                string releaseInfoJson = await client.GetStringAsync(Constants.ApiRepoUrl);
                string latestSetupUrl = releaseInfoJson.Split(new string[] { "\"browser_download_url\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim().Replace("\"", "").TrimEnd('}');
                string setupFileName = Path.Combine(Path.GetTempPath(), "PoliceReportSetup.exe");
                Uri uri = new Uri(latestSetupUrl);

                // Téléchargement de la mise à jour
                using (var downloadClient = new HttpClient())
                {
                    var response = await downloadClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
                    var canReportProgress = totalBytes != -1;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(setupFileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                                progressBar.Value = 100;
                                continue;
                            }

                            await fileStream.WriteAsync(buffer, 0, read);

                            totalRead += read;
                            if (canReportProgress)
                            {
                                progressBar.Value = (totalRead * 100) / totalBytes;
                            }
                        }
                        while (isMoreToRead);
                    }
                }

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
