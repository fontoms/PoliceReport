using LogicLayer.Patrouille;
using LogicLayer.Personne;
using LogicLayer.Specialisation;
using LogicLayer.Unite;
using LogicLayer.Vehicule;
using StorageLayer.Dao;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PoliceReport.Views
{
    /// <summary>
    /// Logique d'interaction pour AjoutPatrouilleWindow.xaml
    /// </summary>
    public partial class AjoutPatrouilleWindow : Window
    {
        public static ObservableCollection<Personne> Effectifs { get; set; }
        private Patrouille _selectedItem;

        public AjoutPatrouilleWindow(Patrouille selectedItem)
        {
            InitializeComponent();
            LoadIndicatifs();
            LoadVehicules();

            _selectedItem = selectedItem;

            Effectifs = new ObservableCollection<Personne>();
            DataContext = this;

            if (_selectedItem != null)
            {
                // Remplir les contrôles avec les informations de la patrouille sélectionnée
                if (_selectedItem.Indicatif != null)
                {
                    foreach (var item in indicatifComboBox.Items)
                    {
                        if (((ComboBoxItem)item).Content.ToString() == _selectedItem.Indicatif.Nom)
                        {
                            indicatifComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (_selectedItem.Vehicule != null)
                {
                    foreach (var item in vehiculeComboBox.Items)
                    {
                        if (((ComboBoxItem)item).Content.ToString() == _selectedItem.Vehicule.Nom)
                        {
                            vehiculeComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (_selectedItem.Effectifs != null)
                {
                    foreach (Personne personne in _selectedItem.Effectifs)
                    {
                        Effectifs.Add(personne);
                    }
                }

                if (_selectedItem.Effectifs.Count > 1)
                {
                    // Changer le titre de la fenêtre
                    Title = "Modifier une patrouille";
                    Valider.Content = "Modifier";
                }
            }
            else
            {
                _selectedItem = new Patrouille();
            }
        }

        private void LoadIndicatifs()
        {
            indicatifComboBox.Items.Clear();

            SpecialisationsDao specialisationsDao = new SpecialisationsDao();
            List<Specialisation> specialisations = specialisationsDao.GetAll();
            foreach (Specialisation specialisation in specialisations)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = specialisation.Nom;
                comboBoxItem.FontWeight = FontWeights.Bold;
                comboBoxItem.IsEnabled = false;
                indicatifComboBox.Items.Add(comboBoxItem);

                UnitesDao unitesDao = new UnitesDao();
                List<Unite> unites = unitesDao.GetAllBySpecialisation(specialisation.Type);
                foreach (Unite unite in unites)
                {
                    ComboBoxItem uniteItem = new ComboBoxItem();
                    uniteItem.Content = unite.Nom;
                    uniteItem.Tag = unite.Type;
                    indicatifComboBox.Items.Add(uniteItem);
                }
            }

            if (indicatifComboBox.Items.Count > 1)
            {
                indicatifComboBox.SelectedIndex = 1;
            }
            else
            {
                indicatifComboBox.IsEnabled = false;
            }
        }

        private void LoadVehicules()
        {
            vehiculeComboBox.Items.Clear();

            SpecialisationsDao specialisationsDao = new SpecialisationsDao();
            List<Specialisation> specialisations = specialisationsDao.GetAll();
            foreach (Specialisation specialisation in specialisations)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = specialisation.Nom;
                comboBoxItem.FontWeight = FontWeights.Bold;
                comboBoxItem.IsEnabled = false;
                vehiculeComboBox.Items.Add(comboBoxItem);

                VehiculesDao vehiculesDao = new VehiculesDao();
                List<Vehicule> vehicules = vehiculesDao.GetAllBySpecialisation(specialisation.Type);
                foreach (Vehicule vehicule in vehicules)
                {
                    ComboBoxItem vehiculeItem = new ComboBoxItem();
                    vehiculeItem.Content = vehicule.Nom;
                    vehiculeComboBox.Items.Add(vehiculeItem);
                }
            }

            if (vehiculeComboBox.Items.Count > 1)
            {
                vehiculeComboBox.SelectedIndex = 1;
            }
            else
            {
                vehiculeComboBox.IsEnabled = false;
            }
        }

        private void AjouterEffectif_Click(object sender, RoutedEventArgs e)
        {
            AjoutPersonneWindow ajoutPersonneWindow = new AjoutPersonneWindow(null);
            ajoutPersonneWindow.Owner = this;
            ajoutPersonneWindow.ShowDialog();
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les informations de la patrouille depuis les contrôles
            UnitesDao unitesDao = new UnitesDao();
            Unite indicatif = unitesDao.GetAllByNom(((ComboBoxItem)indicatifComboBox.SelectedItem).Content.ToString()).First();
            VehiculesDao vehiculesDao = new VehiculesDao();
            Vehicule vehicule = vehiculesDao.GetAllByNom(((ComboBoxItem)vehiculeComboBox.SelectedItem).Content.ToString()).First();

            // Mettre à jour les informations de la patrouille sélectionnée
            _selectedItem.Indicatif = indicatif;
            _selectedItem.Vehicule = vehicule;
            _selectedItem.Effectifs = Effectifs.ToList();
            _selectedItem.HeureDebutPatrouille = _selectedItem.Id == 0 ? DateTime.Now : _selectedItem.HeureDebutPatrouille;

            // Ajouter/Modifier la patrouille à la liste
            if (_selectedItem.Id == 0)
            {
                MainWindow.AddPatrouille(_selectedItem);
            }
            else
            {
                MainWindow.EditPatrouille(_selectedItem);
            }

            // Fermer la fenêtre d'ajout de patrouille
            this.Close();
        }

        public static void AddEffectif(Personne personne)
        {
            Effectifs.Add(personne);
        }

        public static void EditEffectif(Personne personne)
        {
            int index = Effectifs.IndexOf(Effectifs.First(p => p.Id == personne.Id));
            Effectifs[index] = personne;
        }

        private void SupprimerEffectif_Click(object sender, RoutedEventArgs e)
        {
            if (effectifsListBox.SelectedItem != null)
            {
                Effectifs.Remove((Personne)effectifsListBox.SelectedItem);
            }
        }

        private void ModifierEffectif_Click(object sender, RoutedEventArgs e)
        {
            if (effectifsListBox.SelectedItem != null)
            {
                AjoutPersonneWindow ajoutPersonneWindow = new AjoutPersonneWindow((Personne)effectifsListBox.SelectedItem);
                ajoutPersonneWindow.Owner = this;
                ajoutPersonneWindow.ShowDialog();
            }
        }
    }
}
