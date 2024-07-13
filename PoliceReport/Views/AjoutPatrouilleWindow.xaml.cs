using LogicLayer.Effectif;
using LogicLayer.Patrouille;
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
        private bool _isDisplayList = true;
        private Patrouille _selectedItem;

        public static ObservableCollection<Effectif> Effectifs { get; set; }

        public AjoutPatrouilleWindow(Patrouille selectedItem)
        {
            InitializeComponent();
            LoadIndicatifs();
            if (_isDisplayList)
            {
                LoadVehicules();
            }

            _selectedItem = selectedItem;

            Effectifs = new ObservableCollection<Effectif>();
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
                    foreach (Effectif personne in _selectedItem.Effectifs)
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

            List<Specialisation> specialisations = SpecialisationsDao.Instance.GetAll();
            foreach (Specialisation specialisation in specialisations)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = specialisation.Nom;
                comboBoxItem.FontWeight = FontWeights.Bold;
                comboBoxItem.IsEnabled = false;
                indicatifComboBox.Items.Add(comboBoxItem);

                List<Unite> unites = UnitesDao.Instance.GetAllBySpecialisation(specialisation.Type);
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

            // Vérifiez si l'élément sélectionné est une unité
            if (_isDisplayList)
            {
                DisplayVehiculeByList();
            }
            else
            {
                DisplayVehiculeByIndicatif();
            }

            if (_isDisplayList && vehiculeComboBox.Items.Count > 1)
            {
                vehiculeComboBox.SelectedIndex = 1;
            }
            else if (!_isDisplayList && vehiculeComboBox.Items.Count > 0)
            {
                vehiculeComboBox.SelectedIndex = 0;
            }
            else
            {
                vehiculeComboBox.IsEnabled = false;
            }
        }

        private void DisplayVehiculeByList()
        {
            List<Specialisation> specialisations = SpecialisationsDao.Instance.GetAll();

            ChargementWindow chargementVehicules = new ChargementWindow("Chargement des véhicules...");
            chargementVehicules.Show();
            chargementVehicules.MaxValue = specialisations.Count;

            foreach (Specialisation specialisation in specialisations)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = specialisation.Nom;
                comboBoxItem.FontWeight = FontWeights.Bold;
                comboBoxItem.IsEnabled = false;
                vehiculeComboBox.Items.Add(comboBoxItem);

                List<Vehicule> vehicules = VehiculesDao.Instance.GetAllBySpecialisation(specialisation.Type);
                foreach (Vehicule vehicule in vehicules)
                {
                    ComboBoxItem vehiculeItem = new ComboBoxItem();
                    vehiculeItem.Content = vehicule.Nom;
                    vehiculeComboBox.Items.Add(vehiculeItem);
                }

                chargementVehicules.ProgressValue++;
            }

            chargementVehicules.Close();
        }

        private void DisplayVehiculeByIndicatif()
        {
            ChargementWindow chargementVehicules = new ChargementWindow("Chargement des véhicules...");
            chargementVehicules.Show();

            if (((ComboBoxItem)indicatifComboBox.SelectedItem).Tag != null)
            {
                // Obtenez le type d'unité
                string typeUnite = ((ComboBoxItem)indicatifComboBox.SelectedItem).Tag.ToString();

                Unite unite = UnitesDao.Instance.GetType(typeUnite);

                // Si l'unité est spécialisée dans OPJ, chargez tous les véhicules contenant "bana" dans le nom
                if (unite.UnitSpecialisation == "OPJ")
                {
                    List<Vehicule> vehicules = VehiculesDao.Instance.GetAllByNameContains("bana");

                    chargementVehicules.MaxValue = vehicules.Count;

                    foreach (Vehicule vehicule in vehicules)
                    {
                        ComboBoxItem vehiculeItem = new ComboBoxItem();
                        vehiculeItem.Content = vehicule.Nom;
                        vehiculeComboBox.Items.Add(vehiculeItem);

                        chargementVehicules.ProgressValue++;
                    }
                }
                else
                {
                    // Si ce n'est pas une unité spécialisée dans OPJ, chargez les véhicules correspondants à cette unité
                    List<Vehicule> vehicules = VehiculesDao.Instance.GetAllBySpecialisation(unite.UnitSpecialisation);

                    chargementVehicules.MaxValue = vehicules.Count;

                    foreach (Vehicule vehicule in vehicules)
                    {
                        ComboBoxItem vehiculeItem = new ComboBoxItem();
                        vehiculeItem.Content = vehicule.Nom;
                        vehiculeComboBox.Items.Add(vehiculeItem);

                        chargementVehicules.ProgressValue++;
                    }
                }
            }
            else
            {
                List<Specialisation> specialisations = SpecialisationsDao.Instance.GetAll();

                chargementVehicules.MaxValue = specialisations.Count;

                foreach (Specialisation specialisation in specialisations)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = specialisation.Nom;
                    comboBoxItem.FontWeight = FontWeights.Bold;
                    comboBoxItem.IsEnabled = false;
                    vehiculeComboBox.Items.Add(comboBoxItem);

                    List<Vehicule> vehicules = VehiculesDao.Instance.GetAllBySpecialisation(specialisation.Type);
                    foreach (Vehicule vehicule in vehicules)
                    {
                        ComboBoxItem vehiculeItem = new ComboBoxItem();
                        vehiculeItem.Content = vehicule.Nom;
                        vehiculeComboBox.Items.Add(vehiculeItem);
                    }

                    chargementVehicules.ProgressValue++;
                }
            }

            chargementVehicules.Close();
        }

        private void AjouterEffectif_Click(object sender, RoutedEventArgs e)
        {
            AjoutEffectifWindow ajoutPersonneWindow = new AjoutEffectifWindow(null);
            ajoutPersonneWindow.Owner = this;
            ajoutPersonneWindow.ShowDialog();
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les informations de la patrouille depuis les contrôles
            Unite indicatif = UnitesDao.Instance.GetAllByNom(((ComboBoxItem)indicatifComboBox.SelectedItem).Content.ToString()).First();
            Vehicule vehicule = VehiculesDao.Instance.GetAllByNom(((ComboBoxItem)vehiculeComboBox.SelectedItem).Content.ToString()).First();

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
            Close();
        }

        public static void AddEffectif(Effectif personne)
        {
            Effectifs.Add(personne);
        }

        public static void EditEffectif(Effectif personne)
        {
            int index = Effectifs.IndexOf(Effectifs.First(p => p.IdDiscord == personne.IdDiscord));
            Effectifs[index] = personne;
        }

        private void SupprimerEffectif_Click(object sender, RoutedEventArgs e)
        {
            if (effectifsListBox.SelectedItem != null)
            {
                Effectifs.Remove((Effectif)effectifsListBox.SelectedItem);
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une personne à supprimer.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ModifierEffectif_Click(object sender, RoutedEventArgs e)
        {
            if (effectifsListBox.SelectedItem != null)
            {
                AjoutEffectifWindow ajoutPersonneWindow = new AjoutEffectifWindow((Effectif)effectifsListBox.SelectedItem);
                ajoutPersonneWindow.Owner = this;
                ajoutPersonneWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une personne à modifier.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void indicatifComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isDisplayList)
            {
                LoadVehicules();
            }
        }
    }
}
