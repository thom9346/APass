using APass.Core.Entities;
using APass.Core.Interfaces;
using APass.Core.Services;
using APass.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for AddPasswordWindow.xaml
    /// </summary>
    public partial class AddPasswordWindow : Window
    {
        private readonly ICryptographicManager _cryptoManager;
        private IRepository<PasswordEntry> _passwordEntryRepository;

        public PasswordEntry NewPasswordEntry { get; private set; }
        public AddPasswordWindow(ICryptographicManager cryptoManager, IRepository<PasswordEntry> passwordEntryRepository)
        {
            _cryptoManager = cryptoManager;
            _passwordEntryRepository = passwordEntryRepository;
            InitializeComponent();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var website = WebsiteTextBox.Text;
                var username = UsernameTextBox.Text;
                var plaintextPassword = PasswordBox.Password;

                // Assuming you retrieve the DEK securely when the window is opened or when needed
                var dek = SecureSessionService.GetDEK(); // Retrieve DEK securely

                // Convert the plaintext password to a byte array
                var plaintextBytes = Encoding.UTF8.GetBytes(plaintextPassword);

                // Encrypt the password
                var encryptedPasswordWithIV = _cryptoManager.Encrypt(plaintextBytes, dek);

                // Convert encrypted byte array to a base64 string for storage
                var encryptedPasswordBase64 = Convert.ToBase64String(encryptedPasswordWithIV);

                NewPasswordEntry = new PasswordEntry
                {
                    Website = website,
                    Username = username,
                    Password = encryptedPasswordBase64
                };

                _passwordEntryRepository.Add(NewPasswordEntry);
                this.DialogResult = true; // Indicate successful entry
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VisiblePasswordBox.Text = PasswordBox.Password;
            VisiblePasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Hidden;
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            VisiblePasswordBox.Visibility = Visibility.Hidden;
            PasswordBox.Visibility = Visibility.Visible;
        }

        private void GeneratePassword(object sender, RoutedEventArgs e)
        {
            //fixed size of 16 is probably secure enough, but this adds some entropy i guess
            Random rdm = new Random();
            var rdmLength = rdm.Next(16, 64 + 1);

            PasswordBox.Password = GenerateSecurePassword(rdmLength);
        }
        private string GenerateSecurePassword(int length)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "1234567890";
            const string symbols = "!@#$%^&*()";
            string validChars = letters.ToUpper() + letters + digits + symbols;

            StringBuilder password = new StringBuilder();
            byte[] randomBytes = _cryptoManager.CryptographicRandomNumberGenerator(length);


            for (int i = 0; i < length; i++)
            {
                password.Append(validChars[randomBytes[i] % validChars.Length]);
            }
            return password.ToString();
        }

        private void CopyPassword(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(PasswordBox.Password);
        }
    }
}
