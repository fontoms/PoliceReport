using Microsoft.Extensions.DependencyInjection;
using PoliceReport.Core.Effectif;
using PoliceReport.Core.Grade;
using PoliceReport.Core.PositionVeh;
using System.Windows;

namespace PoliceReport.Views
{
    public partial class AjoutEffectifWindow : Window
    {
        private Effectif _selectedItem;

        private readonly IGradeDao _gradeDao;
        private readonly IEffectifDao _effectifDao;

        public AjoutEffectifWindow(Effectif selectedItem, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _selectedItem = selectedItem;

            // Récupérer les DAO à partir du conteneur DI
            _effectifDao = serviceProvider.GetRequiredService<IEffectifDao>();
            _gradeDao = serviceProvider.GetRequiredService<IGradeDao>();

            LoadEffectifs();
            LoadGrades();
            LoadVehPositions();
            idTextBox.Focus();

            if (_selectedItem != null)
            {
                // Remplir les contrôles avec les informations de la personne sélectionnée
                idTextBox.Text = _selectedItem.Id.ToString();
                foreach (var effectif in effectifComboBox.Items)
                {
                    if (((Effectif)effectif).Id == _selectedItem.Id)
                    {
                        effectifComboBox.SelectedItem = effectif;
                        break;
                    }
                }
                foreach (var grade in gradeComboBox.Items)
                {
                    if (((Grade)grade).Id == _selectedItem.Grade.Id)
                    {
                        gradeComboBox.SelectedItem = grade;
                        break;
                    }
                }
                positionComboBox.SelectedItem = _selectedItem.PositionVehicule;

                // Changer le titre de la fenêtre
                Title = "Modifier une personne";
                AddEditPersonn.Content = "Modifier";
            }
            else
            {
                _selectedItem = new Effectif();
            }
        }

        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les informations de la personne depuis les contrôles
            _selectedItem = (Effectif)effectifComboBox.SelectedItem;
            //_selectedItem.Grade = (Grade)gradeComboBox.SelectedItem;
            _selectedItem.PositionVehicule = positionComboBox.SelectedItem.ToString();

            // Ajouter/Modifier la personne à la liste
            if (AjoutPatrouilleWindow.Effectifs.All(p => p.Id != _selectedItem.Id))
            {
                AjoutPatrouilleWindow.AddEffectif(_selectedItem);
            }
            else
            {
                AjoutPatrouilleWindow.EditEffectif(_selectedItem);
            }

            // Fermer la fenêtre d'ajout de personne
            Close();
        }

        private void LoadEffectifs()
        {
            List<Effectif> effectifs = _effectifDao.GetAllEffectifs();
            List<Grade> grades = _gradeDao.GetAll();
            foreach (Effectif effectif in effectifs)
            {
                effectif.Grade = grades.Find(g => g.Id == effectif.EffGrade);
            }
            effectifComboBox.ItemsSource = effectifs;

            if (effectifComboBox.Items.Count > 0)
            {
                effectifComboBox.SelectedIndex = 0;
            }
            else
            {
                effectifComboBox.IsEnabled = false;
            }
        }

        private void LoadGrades()
        {
            gradeComboBox.Items.Clear();
            gradeComboBox.ItemsSource = _gradeDao.GetAll();

            if (gradeComboBox.Items.Count > 0)
            {
                gradeComboBox.SelectedIndex = 0;
            }
            else
            {
                gradeComboBox.IsEnabled = false;
            }
        }

        private void LoadVehPositions()
        {
            positionComboBox.Items.Clear();
            foreach (string position in new string[] { PositionVeh.ChefDeBord, PositionVeh.Conducteur, PositionVeh.Equipier })
            {
                positionComboBox.Items.Add(position);
            }

            if (positionComboBox.Items.Count > 0)
            {
                positionComboBox.SelectedIndex = 0;
            }
        }
    }
}
