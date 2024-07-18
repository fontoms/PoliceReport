using LogicLayer.Effectif;
using LogicLayer.Grade;
using LogicLayer.Infraction;
using LogicLayer.Outils;
using LogicLayer.Patrouille;
using LogicLayer.PositionVeh;
using StorageLayer;
using StorageLayer.Dao;
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
        private static int _lastPatrouilleId = 0;
        private int _lastActionId = 0;
        private const string _searchInfractionDefaultText = "Rechercher une infraction ou action...";

        public static ObservableCollection<Patrouille> Patrouilles { get; set; }
        public static ObservableCollection<LogicLayer.Action.Action> Actions { get; set; }
        private ChargementWindow _chargementWindow;

        public MainWindow()
        {
            InitializeComponent();

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
            Actions = new ObservableCollection<LogicLayer.Action.Action>();

            // Assigner la liste des personnes au DataContext de la fenêtre
            DataContext = this;

            // Charger les autres éléments une fois que la base de données est prête
            LoadAllElements();
            effectifComboBox.Focus();
        }

        private void LoadEffectifs()
        {
            effectifComboBox.ItemsSource = null;

            List<Effectif> effectifs = EffectifsDao.Instance.GetAllEffectifs();
            List<Grade> grades = GradesDao.Instance.GetAll();
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

            List<LogicLayer.Action.Action> actionsList = new List<LogicLayer.Action.Action>();
            List<Infraction> infractions = InfractionsDao.Instance.GetAll();

            _chargementWindow.MaxValue = infractions.Count;

            foreach (Infraction infraction in infractions)
            {
                actionsList.Add(new LogicLayer.Action.Action(infraction.Nom, DateTime.Now));

                List<LogicLayer.Action.Action> actions = ActionsDao.Instance.GetAllByInfractions(infraction.Id);
                foreach (LogicLayer.Action.Action action in actions)
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
            if (actionsListBox.SelectedItem != null && ((LogicLayer.Action.Action)actionsListBox.SelectedItem).ActInfraction != 0 && !startServiceBtn.IsEnabled)
            {
                LogicLayer.Action.Action action = new LogicLayer.Action.Action(_lastActionId++, ((LogicLayer.Action.Action)actionsListBox.SelectedItem).Nom, DateTime.Now, ((LogicLayer.Action.Action)actionsListBox.SelectedItem).ActInfraction);
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
            _lastPatrouilleId++;
            patrouille.Id = _lastPatrouilleId;
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
            });
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
                AjoutPatrouilleWindow ajoutPatrouilleWindow = new AjoutPatrouilleWindow((Patrouille)patrouilleListBox.SelectedItem);
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
            if (selectedActionsListBox.SelectedItem != null && ((LogicLayer.Action.Action)selectedActionsListBox.SelectedItem).ActInfraction != 0)
            {
                Actions.Remove((LogicLayer.Action.Action)selectedActionsListBox.SelectedItem);
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
                    LogicLayer.Action.Action action = (LogicLayer.Action.Action)item;
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
            foreach (LogicLayer.Action.Action description in selectedActionsListBox.Items)
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
            ConnexionWindow connexionWindow = new ConnexionWindow();
            connexionWindow.Owner = this;
            connexionWindow.ShowDialog();

            if (connexionWindow.MotDePasseCorrect)
            {
                AdministrationWindow administrationWindow = new AdministrationWindow();
                administrationWindow.User = connexionWindow.User;
                administrationWindow.Show();
                administrationWindow.Closed += (obj, arg) =>
                {
                    LoadAllElements();
                };
            }
        }

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            _chargementWindow = new ChargementWindow("Mise à jour de PoliceReport...");
            _chargementWindow.Show();

            try
            {
                BaseDao database = new BaseDao();
                database.ProgressChanged += (sender, e) =>
                {
                    _chargementWindow.ProgressValue = e;
                };
                string version = await database.Update();

                if (version != null)
                {
                    _chargementWindow.Close();
                    LoadAllElements();
                    MessageBox.Show("La base de données a été mise à jour avec succès.\n\nNouvelle version : " + version, "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _chargementWindow.Close();
                    MessageBox.Show("La base de données est déjà à jour.", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                _chargementWindow.Close();
                MessageBox.Show("Erreur lors de la mise à jour de la base de données :\n" + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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