using Microsoft.Extensions.DependencyInjection;
using PoliceReport.Core.Action;
using PoliceReport.Core.Effectif;
using PoliceReport.Core.Grade;
using PoliceReport.Core.Infraction;
using PoliceReport.Core.Patrouille;
using PoliceReport.Core.PositionVeh;
using PoliceReport.Core.Tools;
using PoliceReport.Core.Tools.Updates;
using PoliceReport.Updates;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PoliceReport.Views
{
    public partial class MainWindow : Window
    {
        private static int LastPatrouilleId = 0;
        private int _lastActionId = 0;
        private const string _searchInfractionDefaultText = "Rechercher une infraction ou action...";

        public static ObservableCollection<Patrouille> Patrouilles { get; set; }
        public static ObservableCollection<PoliceReport.Core.Action.Action> Actions { get; set; }
        private ChargementWindow _chargementWindow;

        private readonly IEffectifDao _effectifDao;
        private readonly IGradeDao _gradeDao;
        private readonly IInfractionDao _infractionDao;
        private readonly IActionDao _actionDao;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Récupérer les DAO à partir du conteneur DI
            _effectifDao = serviceProvider.GetRequiredService<IEffectifDao>();
            _gradeDao = serviceProvider.GetRequiredService<IGradeDao>();
            _infractionDao = serviceProvider.GetRequiredService<IInfractionDao>();
            _actionDao = serviceProvider.GetRequiredService<IActionDao>();

            versionAuthorLbl.Content = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " - " + Constants.Author;
            versionAuthorLbl.MouseLeftButtonDown += (sender, e) =>
            {
                // Ouvrir le lien URL lorsque le label est cliqué
                string url = Constants.RepoUrl;
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            };

            // Initialiser la liste des personnes
            Patrouilles = new ObservableCollection<Patrouille>();

            // Initialiser la liste des infractions
            Actions = new ObservableCollection<PoliceReport.Core.Action.Action>();

            // Assigner la liste des personnes au DataContext de la fenêtre
            DataContext = this;

            // Charger les autres éléments une fois que la base de données est prête
            LoadAllElements();
            effectifComboBox.Focus();
        }

        private void LoadEffectifs()
        {
            effectifComboBox.ItemsSource = null;

            List<Effectif> effectifs = _effectifDao.GetAllEffectifs();
            List<Grade> grades = _gradeDao.GetAll();
            foreach (Effectif effectif in effectifs)
            {
                effectif.Grade = grades.FirstOrDefault(g => g.Id == effectif.EffGrade);
            }
            effectifComboBox.ItemsSource = effectifs;

            if (effectifComboBox.Items.Count > 0)
            {
                effectifComboBox.SelectedIndex = 0;
                effectifComboBox.IsEnabled = true;
            }
            else
            {
                effectifComboBox.IsEnabled = false;
            }
        }

        private void LoadActions()
        {
            actionsListBox.ItemsSource = null;

            _chargementWindow = new ChargementWindow("Chargement des actions...");
            _chargementWindow.Show();

            List<PoliceReport.Core.Action.Action> actionsList = new List<PoliceReport.Core.Action.Action>();
            List<Infraction> infractions = _infractionDao.GetAll();

            _chargementWindow.MaxValue = infractions.Count;

            foreach (Infraction infraction in infractions)
            {
                actionsList.Add(new PoliceReport.Core.Action.Action(infraction.Nom, DateTime.Now));

                List<PoliceReport.Core.Action.Action> actions = _actionDao.GetAllByInfractions(infraction.Id);
                foreach (PoliceReport.Core.Action.Action action in actions)
                {
                    action.ActInfraction = infraction.Id;
                    actionsList.Add(action);
                }

                _chargementWindow.ProgressValue++;
            }

            _chargementWindow.Close();
            actionsListBox.ItemsSource = actionsList;
        }

        private void ActionsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ajouter l'infraction sélectionnée à la liste des infractions
            if (actionsListBox.SelectedItem != null && ((PoliceReport.Core.Action.Action)actionsListBox.SelectedItem).ActInfraction != 0 && !startServiceBtn.IsEnabled)
            {
                PoliceReport.Core.Action.Action action = new PoliceReport.Core.Action.Action(_lastActionId++, ((PoliceReport.Core.Action.Action)actionsListBox.SelectedItem).Nom, DateTime.Now, ((PoliceReport.Core.Action.Action)actionsListBox.SelectedItem).ActInfraction);
                Actions.Add(action);
            }
        }

        public static void AddPatrouille(Patrouille patrouille)
        {
            if (Patrouilles.Count > 0)
            {
                Patrouille patrouillePrecedente = Patrouilles[Patrouilles.Count - 1];
                patrouillePrecedente.HeureFinPatrouille = DateTime.Now;
            }
            LastPatrouilleId++;
            patrouille.Id = LastPatrouilleId;
            Patrouilles.Add(patrouille);
        }

        public static void EditPatrouille(Patrouille patrouille)
        {
            Patrouille patrouilleToEdit = Patrouilles.FirstOrDefault(p => p.Id == patrouille.Id);
            if (patrouilleToEdit != null)
            {
                patrouilleToEdit.Indicatif = patrouille.Indicatif;
                patrouilleToEdit.Vehicule = patrouille.Vehicule;
                patrouilleToEdit.Effectifs = patrouille.Effectifs;

                // Notifier la modification de la patrouille à l'interface utilisateur
                int index = Patrouilles.IndexOf(patrouilleToEdit);
                Patrouilles[index] = patrouilleToEdit;
            }
        }

        private void AddPatrouille_Click(object sender, RoutedEventArgs e)
        {
            Effectif effectif = (Effectif)effectifComboBox.SelectedItem;
            effectif.PositionVehicule = PositionVeh.ChefDeBord;
            AjoutPatrouilleWindow ajoutPatrouilleWindow = new AjoutPatrouilleWindow(new Patrouille
            {
                Effectifs = new List<Effectif> { effectif },
            }, Startup.ConfigureServices());
            ajoutPatrouilleWindow.Owner = this;
            ajoutPatrouilleWindow.ShowDialog();

        }

        private void DeletePatrouille_Click(object sender, RoutedEventArgs e)
        {
            if (patrouilleListBox.SelectedItem != null)
            {
                Patrouille patrouille = (Patrouille)patrouilleListBox.SelectedItem;
                Patrouilles.Remove(patrouille);
                if (Patrouilles.Count > 0)
                {
                    Patrouille patrouillePrecedente = Patrouilles[Patrouilles.Count - 1];
                    patrouillePrecedente.HeureFinPatrouille = null;
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une patrouille à supprimer.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditPatrouille_Click(object sender, RoutedEventArgs e)
        {
            if (patrouilleListBox.SelectedItem != null)
            {
                AjoutPatrouilleWindow ajoutPatrouilleWindow = new AjoutPatrouilleWindow((Patrouille)patrouilleListBox.SelectedItem, Startup.ConfigureServices());
                ajoutPatrouilleWindow.Owner = this;
                ajoutPatrouilleWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une patrouille à modifier.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SelectedActionsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Supprimer l'infraction sélectionnée de la liste des infractions
            if (selectedActionsListBox.SelectedItem != null && ((PoliceReport.Core.Action.Action)selectedActionsListBox.SelectedItem).ActInfraction != 0)
            {
                Actions.Remove((PoliceReport.Core.Action.Action)selectedActionsListBox.SelectedItem);
            }
        }

        private void SearchInfractionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Filtrer la liste des infractions
            if (string.IsNullOrWhiteSpace(searchInfractionTextBox.Text))
            {
                // Remettre la liste dans l'état par défaut
                actionsListBox.Items.Filter = null;
            }
            else if (searchInfractionTextBox.Text != _searchInfractionDefaultText)
            {
                // Convertir le texte en minuscules
                string searchText = searchInfractionTextBox.Text.ToLower();
                string searchTextNormalized = NormalizeText(searchText);
                actionsListBox.Items.Filter = item =>
                {
                    PoliceReport.Core.Action.Action action = (PoliceReport.Core.Action.Action)item;
                    return action.ActInfraction != 0 && NormalizeText(action.Nom.ToLower()).Contains(searchTextNormalized);
                };
            }
        }

        private string NormalizeText(string text)
        {
            // Remplace les caractères accentués par leur équivalent non accentué
            return text.Normalize(NormalizationForm.FormD).Replace('\u0300', '\u0065');
        }

        private void SearchInfractionTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Effacer le texte par défaut
            if (searchInfractionTextBox.Text == _searchInfractionDefaultText)
            {
                searchInfractionTextBox.Text = "";
            }
        }

        private void SearchInfractionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Remettre le texte par défaut
            if (string.IsNullOrWhiteSpace(searchInfractionTextBox.Text))
            {
                searchInfractionTextBox.Text = _searchInfractionDefaultText;
            }
        }

        private void GenererRapportButton_Click(object sender, RoutedEventArgs e)
        {
            // Générer le rapport
            StringBuilder rapport = new StringBuilder();
            rapport.AppendLine("# RAPPORT DE PATROUILLE");
            rapport.AppendLine("**DATE :** " + DateTime.Now.ToString("dd/MM/yyyy"));
            rapport.AppendLine();
            rapport.AppendLine(startServiceLbl.Content.ToString());
            rapport.AppendLine();
            foreach (Patrouille patrouille in Patrouilles)
            {
                rapport.AppendLine("__Indicatif :__ **" + patrouille.Indicatif.Nom + "**");
                rapport.AppendLine(" - Véhicule : " + patrouille.Vehicule.Nom);
                rapport.AppendLine(" - Début : " + patrouille.HeureDebutPatrouille.ToString("HH:mm"));
                rapport.AppendLine(" - Fin : " + patrouille.HeureFinPatrouille.Value.ToString("HH:mm"));
                rapport.AppendLine("  - Effectifs :");
                foreach (Effectif effectif in patrouille.Effectifs)
                {
                    rapport.AppendLine("   - " + effectif.PositionVehicule + " : <@" + effectif.IdDiscord + ">");
                }
                rapport.AppendLine();
            }
            rapport.AppendLine("__Description :__");
            foreach (PoliceReport.Core.Action.Action description in selectedActionsListBox.Items)
            {
                rapport.AppendLine("- " + description.Heure.ToString("HH:mm") + " : " + description.Nom);
            }
            rapport.AppendLine();
            rapport.AppendLine(endServiceLbl.Content.ToString());

            // Copier le rapport dans le presse-papier
            Clipboard.SetText(rapport.ToString());

            // Message de confirmation
            MessageBox.Show("Le rapport a été généré et copié dans le presse-papier.", "Rapport généré", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StartServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            effectifComboBox.IsEnabled = false;
            updateBtn.IsEnabled = false;
            startServiceLbl.Content = "Prise de service : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            startServiceBtn.IsEnabled = !startServiceBtn.IsEnabled;
            endServiceBtn.IsEnabled = !endServiceBtn.IsEnabled;
            AddPatrouilleBtn.IsEnabled = !AddPatrouilleBtn.IsEnabled;
            EditPatrouilleBtn.IsEnabled = !EditPatrouilleBtn.IsEnabled;
            DeletePatrouilleBtn.IsEnabled = !DeletePatrouilleBtn.IsEnabled;
        }

        private void EndServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            endServiceLbl.Content = "Fin de service : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            endServiceBtn.IsEnabled = !endServiceBtn.IsEnabled;
            genererRapportButton.IsEnabled = !genererRapportButton.IsEnabled;
            restartServiceBtn.IsEnabled = !restartServiceBtn.IsEnabled;
            if (Patrouilles.Count > 0)
            {
                Patrouille patrouillePrecedente = Patrouilles[Patrouilles.Count - 1];
                patrouillePrecedente.HeureFinPatrouille = DateTime.Now;
            }
        }

        private void RestartServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            startServiceLbl.Content = "Hors service";
            startServiceBtn.IsEnabled = true;
            endServiceLbl.Content = "Hors service";
            endServiceBtn.IsEnabled = false;
            genererRapportButton.IsEnabled = false;
            restartServiceBtn.IsEnabled = false;
            effectifComboBox.IsEnabled = true;
            updateBtn.IsEnabled = true;
            Patrouilles.Clear();
            AddPatrouilleBtn.IsEnabled = false;
            EditPatrouilleBtn.IsEnabled = false;
            DeletePatrouilleBtn.IsEnabled = false;
        }

        private void TitleLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConnexionWindow connexionWindow = new ConnexionWindow(Startup.ConfigureServices());
            connexionWindow.Owner = this;
            connexionWindow.ShowDialog();

            if (connexionWindow.MotDePasseCorrect)
            {
                AdministrationWindow administrationWindow = new AdministrationWindow(Startup.ConfigureServices());
                administrationWindow.User = connexionWindow.User;
                administrationWindow.Show();
                administrationWindow.Closed += (obj, arg) =>
                {
                    MainWindow mainWindow = new MainWindow(Startup.ConfigureServices());
                    mainWindow.Show();
                    Close();
                };
            }
        }

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VersionInfo updateAvailable = await Updater.CheckUpdateAvailableAsync();

                if (updateAvailable.LatestVersion == null)
                {
                    // Afficher un avertissement si la vérification des mises à jour a échoué
                    MessageBox.Show("La vérification des mises à jour a échoué. Veuillez réessayer plus tard.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (updateAvailable != null && updateAvailable.LatestVersion > updateAvailable.CurrentVersion)
                {
                    // Si une mise à jour est disponible, ouvrir une boite de dialogue pour demander à l'utilisateur de mettre à jour
                    MessageBoxResult result = MessageBox.Show("Une nouvelle mise à jour est disponible. Voulez-vous la télécharger et l'installer ?", "Mise à jour disponible", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _chargementWindow = new ChargementWindow("Mise à jour de PoliceReport...");
                        _chargementWindow.Show();
                        // Télécharger et installer la mise à jour
                        GitHubVersionProvider versionProvider = new GitHubVersionProvider(Constants.ApiRepoUrl);
                        string downloadUrl = versionProvider.ExtractDownloadUrl(await versionProvider.GetLatestReleaseInfoAsync());
                        WpfUpdateUserInterface uiManager = new WpfUpdateUserInterface(_chargementWindow.progressBar, updateBtn);
                        ApplicationUpdater updater = new ApplicationUpdater(versionProvider, uiManager);
                        UpdateManager updateManager = new UpdateManager(updater);
                        //await updater.DownloadUpdateAsync();
                        await updateManager.PerformUpdateAsync();
                        _chargementWindow.Close();
                    }
                }
                else
                {
                    // Si aucune mise à jour n'est disponible, afficher un message
                    MessageBox.Show("Aucune mise à jour n'est disponible.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                _chargementWindow.Close();
                MessageBox.Show("Erreur lors de la mise à jour :\n" + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllElements()
        {
            try
            {
                LoadEffectifs();
                LoadActions();
                startServiceBtn.IsEnabled = effectifComboBox.IsEnabled;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des éléments :\n\n" + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}