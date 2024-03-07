﻿using APass.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace APass.Wpf
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ICryptographicManager _cryptoManager;

        public LoginWindow(ICryptographicManager cryptoManager)
        {
            _cryptoManager = cryptoManager;
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var masterPassword = PasswordBox.Password;

            // Here, implement your logic to verify the master password.
            // This is a placeholder for the actual verification logic.
            bool isValid = CheckMasterPassword(masterPassword);

            if (isValid)
            {
                // Open the MainWindow and close the LoginWindow
                MainWindow mainWindow = new MainWindow(_cryptoManager);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect Master Password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CheckMasterPassword(string password)
        {
            // Placeholder for actual implementation
            // You would typically hash the input password and compare it with a stored hash
            return true; // Change this to actual verification logic
        }
    }
}
