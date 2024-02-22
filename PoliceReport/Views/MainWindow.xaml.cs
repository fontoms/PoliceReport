using LogicLayer.Grade;
using LogicLayer.Infraction;
using LogicLayer.Patrouille;
using LogicLayer.Personne;
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
        public static BaseDao Database;

        public MainWindow()
        {
            InitializeComponent();

            versionAuthorLbl.Content = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " - Fontom's (677154388065910822)";
            versionAuthorLbl.MouseLeftButtonDown += (sender, e) =>
            {
                // Ouvrir le lien URL lorsque le label est cliqué
                string url = "https://github.com/Fontom71/PoliceReport";
                System.Diagnostics.Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            };

            // Initialiser la liste des personnes
            Patrouilles = new ObservableCollection<Patrouille>();

            // Initialiser la liste des infractions
            Actions = new ObservableCollection<LogicLayer.Action.Action>();

            // Assigner la liste des personnes au DataContext de la fenêtre
            DataContext = this;

            // Initialiser la base de données
            Database = new BaseDao();

            LoadGradeType();
            LoadActions();
            idTextBox.Focus();
        }

        private void LoadGradeType()
        {
            gradeComboBox.Items.Clear();
            GradesDao gradesDao = new GradesDao();
            gradeComboBox.ItemsSource = gradesDao.GetAll();

            if (gradeComboBox.Items.Count > 0)
            {
                gradeComboBox.SelectedIndex = 0;
            }
            else
            {
                gradeComboBox.IsEnabled = false;
            }
        }

        private void LoadActions()
        {
            actionsListBox.Items.Clear();

            ChargementWindow chargementActons = new ChargementWindow("Chargement des actions...");
            chargementActons.Show();

            List<LogicLayer.Action.Action> actionsList = new List<LogicLayer.Action.Action>();
            InfractionsDao infractionsDao = new InfractionsDao();
            List<Infraction> infractions = infractionsDao.GetAll();

            chargementActons.MaxValue = infractions.Count;

            foreach (Infraction infraction in infractions)
            {
                actionsList.Add(new LogicLayer.Action.Action(infraction.Nom, DateTime.Now));

                ActionsDao actionsDao = new ActionsDao();
                List<LogicLayer.Action.Action> actions = actionsDao.GetAllByInfractions(infraction.Nom);
                foreach (LogicLayer.Action.Action action in actions)
                {
                    action.ActInfraction = infraction.Type;
                    actionsList.Add(action);
                }

                chargementActons.ProgressValue++;
            }

            chargementActons.Close();
            actionsListBox.ItemsSource = actionsList;
        }

        private void actionsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ajouter l'infraction sélectionnée à la liste des infractions
            if (actionsListBox.SelectedItem != null && ((LogicLayer.Action.Action)actionsListBox.SelectedItem).ActInfraction != null)
            {
                LogicLayer.Action.Action action = new LogicLayer.Action.Action(((LogicLayer.Action.Action)actionsListBox.SelectedItem).Nom, DateTime.Now);
                _lastActionId++;
                action.Id = _lastActionId;
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
            AjoutPatrouilleWindow ajoutPatrouilleWindow = new AjoutPatrouilleWindow(new Patrouille
            {
                Effectifs = new List<Personne> { new Personne
                {
                    Id = idTextBox.Text,
                    Grade = (Grade)gradeComboBox.SelectedValue,
                    PositionVehicule = PositionVeh.ChefDeBord
                }}
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

        private void selectedActionsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Supprimer l'infraction sélectionnée de la liste des infractions
            if (selectedActionsListBox.SelectedItem != null && ((LogicLayer.Action.Action)selectedActionsListBox.SelectedItem).ActInfraction != null)
            {
                Actions.Remove((LogicLayer.Action.Action)selectedActionsListBox.SelectedItem);
            }
        }

        private void searchInfractionTextBox_TextChanged(object sender, TextChangedEventArgs e)
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
                    return action.ActInfraction != null && NormalizeText(action.Nom.ToLower()).Contains(searchTextNormalized);
                };
            }
        }

        private string NormalizeText(string text)
        {
            // Remplace les caractères accentués par leur équivalent non accentué
            return text.Normalize(NormalizationForm.FormD).Replace('\u0300', '\u0065');
        }

        private void searchInfractionTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Effacer le texte par défaut
            if (searchInfractionTextBox.Text == _searchInfractionDefaultText)
            {
                searchInfractionTextBox.Text = "";
            }
        }

        private void searchInfractionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Remettre le texte par défaut
            if (string.IsNullOrWhiteSpace(searchInfractionTextBox.Text))
            {
                searchInfractionTextBox.Text = _searchInfractionDefaultText;
            }
        }

        private void genererRapportButton_Click(object sender, RoutedEventArgs e)
        {
            // Générer le rapport
            StringBuilder rapport = new StringBuilder();
            rapport.AppendLine("# RAPPORT DE PATROUILLE");
            rapport.AppendLine("**DATE :** " + DateTime.Now.ToString("dd/MM/yyyy"));
            rapport.AppendLine();
            rapport.AppendLine(startServiceLbl.Content.ToString());
            rapport.AppendLine();
            rapport.AppendLine("__Indicatif :__ ");
            foreach (Patrouille patrouille in Patrouilles)
            {
                rapport.AppendLine("- **" + patrouille.Indicatif.Nom + "** (" + patrouille.Vehicule.Nom + ")");
                rapport.AppendLine(" - Début : " + patrouille.HeureDebutPatrouille.ToString("HH:mm"));
                rapport.AppendLine(" - Fin : " + patrouille.HeureFinPatrouille.Value.ToString("HH:mm"));
                rapport.AppendLine("  - Effectifs :");
                foreach (Personne effectif in patrouille.Effectifs)
                {
                    rapport.AppendLine("   - <@" + effectif.Id + "> (" + effectif.PositionVehicule + ")");
                }
            }
            rapport.AppendLine();
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

        private void startServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(idTextBox.Text))
            {
                MessageBox.Show("Veuillez saisir votre identifiant Discord.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            idTextBox.IsEnabled = false;
            gradeComboBox.IsEnabled = false;
            startServiceLbl.Content = "Prise de service : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            startServiceBtn.IsEnabled = !startServiceBtn.IsEnabled;
            endServiceBtn.IsEnabled = !endServiceBtn.IsEnabled;
            AddPatrouilleBtn.IsEnabled = !AddPatrouilleBtn.IsEnabled;
            EditPatrouilleBtn.IsEnabled = !EditPatrouilleBtn.IsEnabled;
            DeletePatrouilleBtn.IsEnabled = !DeletePatrouilleBtn.IsEnabled;
        }

        private void endServiceBtn_Click(object sender, RoutedEventArgs e)
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

        private void restartServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            startServiceLbl.Content = "Hors service";
            startServiceBtn.IsEnabled = true;
            endServiceLbl.Content = "Hors service";
            endServiceBtn.IsEnabled = false;
            genererRapportButton.IsEnabled = false;
            restartServiceBtn.IsEnabled = false;
            idTextBox.IsEnabled = true;
            idTextBox.Text = "";
            gradeComboBox.IsEnabled = true;
            Patrouilles.Clear();
            AddPatrouilleBtn.IsEnabled = false;
            EditPatrouilleBtn.IsEnabled = false;
            DeletePatrouilleBtn.IsEnabled = false;
        }
    }
}