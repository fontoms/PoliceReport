﻿using PoliceReport.Core.Outils;
using PoliceReport.Core.Outils.Cryptage;
using PoliceReport.Core.Utilisateur;
using System.Windows;

namespace PoliceReport.Views
{
    /// <summary>
    /// Logique d'interaction pour ConnexionWindow.xaml
    /// </summary>
    public partial class ConnexionWindow : Window
    {
        public bool MotDePasseCorrect { get; private set; }
        public Utilisateur User { get; private set; }

        public ConnexionWindow()
        {
            InitializeComponent();
            MotDePasseCorrect = false;
            userBox.Focus();
        }

        private void connectionBtn_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Utilisateur user = Constants.Users.FirstOrDefault(u => u.Username == userBox.Text && u.Password == HashHelper.CalculateSHA256(passBox.Password));
#else
            Utilisateur user = UtilisateursDao.Instance.GetUser(userBox.Text, HashHelper.CalculateSHA256(passBox.Password));
#endif
            if (user != null)
            {
                MotDePasseCorrect = true;
                User = user;
                Close();
            }
            else
            {
                MessageBox.Show("Mot de passe incorrect", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
