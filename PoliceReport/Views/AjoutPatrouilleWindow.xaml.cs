using Microsoft.Extensions.DependencyInjection;
using PoliceReport.Core.Effectif;
using PoliceReport.Core.Patrouille;
using PoliceReport.Core.Specialisation;
using PoliceReport.Core.Unite;
using PoliceReport.Core.Vehicule;
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
        private readonly bool _isDisplayList = Settings.Default.VehDisplayList;
        private readonly Patrouille _selectedItem;

        public static ObservableCollection<Effectif> Effectifs { get; set; }

        private readonly ISpecialisationDao _specialisationDao;
        private readonly IUniteDao _uniteDao;
        private readonly IVehiculeDao _vehiculeDao;

        public AjoutPatrouilleWindow(Patrouille selectedItem, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Récupérer les DAO à partir du conteneur DI
            _specialisationDao = serviceProvider.GetRequiredService<ISpecialisationDao>();
            _uniteDao = serviceProvider.GetRequiredService<IUniteDao>();
            _vehiculeDao = serviceProvider.GetRequiredService<IVehiculeDao>();

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

            List<Specialisation> specialisations = _specialisationDao.GetAll();
            foreach (Specialisation specialisation in specialisations)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = specialisation.Nom;
                comboBoxItem.FontWeight = FontWeights.Bold;
                comboBoxItem.IsEnabled = false;
                indicatifComboBox.Items.Add(comboBoxItem);

                List<Unite> unites = _uniteDao.GetAllBySpecialisation(specialisation.Id);
                foreach (Unite unite in unites)
                {
                    ComboBoxItem uniteItem = new ComboBoxItem();
                    uniteItem.Content = unite.Nom;
                    uniteItem.Tag = unite.Id;
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
                vehiculeComboBox.IsEnabled = true;
            }
            else
            {
                vehiculeComboBox.IsEnabled = false;
            }
        }

        private void DisplayVehiculeByList()
        {
            List<Specialisation> specialisations = _specialisationDao.GetAll();

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

                List<Vehicule> vehicules = _vehiculeDao.GetAllBySpecialisation(specialisation.Id);
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

            if ((int)((ComboBoxItem)indicatifComboBox.SelectedItem).Tag != 0)
            {
                // Obtenez le type d'unité
                int idUnite = (int)((ComboBoxItem)indicatifComboBox.SelectedItem).Tag;

                Unite unite = _uniteDao.GetId(idUnite);
                //Specialisation specialisation = SpecialisationsDao.Instance.get

                // Si l'unité est spécialisée dans OPJ, chargez tous les véhicules contenant "bana" dans le nom
                if (unite.UnitSpecialisation == 5)
                {
                    List<Vehicule> vehicules = _vehiculeDao.GetAllByNameContains("bana");

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
                    List<Vehicule> vehicules = _vehiculeDao.GetAllBySpecialisation(unite.UnitSpecialisation);

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
                List<Specialisation> specialisations = _specialisationDao.GetAll();

                chargementVehicules.MaxValue = specialisations.Count;

                foreach (Specialisation specialisation in specialisations)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = specialisation.Nom;
                    comboBoxItem.FontWeight = FontWeights.Bold;
                    comboBoxItem.IsEnabled = false;
                    vehiculeComboBox.Items.Add(comboBoxItem);

                    List<Vehicule> vehicules = _vehiculeDao.GetAllBySpecialisation(specialisation.Id);
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
            AjoutEffectifWindow ajoutPersonneWindow = new AjoutEffectifWindow(null, Startup.ConfigureServices());
            ajoutPersonneWindow.Owner = this;
            ajoutPersonneWindow.ShowDialog();
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les informations de la patrouille depuis les contrôles
            Unite indicatif = _uniteDao.GetAllByNom(((ComboBoxItem)indicatifComboBox.SelectedItem).Content.ToString()).First();
            Vehicule vehicule = _vehiculeDao.GetAllByNom(((ComboBoxItem)vehiculeComboBox.SelectedItem).Content.ToString()).First();

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
                AjoutEffectifWindow ajoutPersonneWindow = new AjoutEffectifWindow((Effectif)effectifsListBox.SelectedItem, Startup.ConfigureServices());
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
