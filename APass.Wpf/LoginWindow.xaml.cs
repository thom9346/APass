using APass.Core.Entities;
using APass.Core.Interfaces;
using APass.Infrastructure;
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
        private IRepository<PasswordEntry> _passwordEntryRepository;
        private readonly PasswordManagerContext _dbContext;

        public LoginWindow(
            ICryptographicManager cryptoManager,
            PasswordManagerContext dbContext,
            IRepository<PasswordEntry> passwordEntryRepository)
        {
            _cryptoManager = cryptoManager;
            _dbContext = dbContext;
            _passwordEntryRepository = passwordEntryRepository;
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var masterPassword = PasswordBox.Password;

            bool isValid = CheckMasterPassword(masterPassword);

            if (isValid)
            {
                MainWindow mainWindow = new MainWindow(_cryptoManager, _dbContext, _passwordEntryRepository);
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
            return true; // Change this to actual verification logic
        }
    }
}
