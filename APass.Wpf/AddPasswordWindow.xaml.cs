using APass.Core.Interfaces;
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
        public AddPasswordWindow(ICryptographicManager cryptoManager)
        {
            _cryptoManager = cryptoManager;
            InitializeComponent();
        }
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            // Here, add logic to store the password information.
            // This could involve updating a collection, a database, etc.
            //string password = PasswordBox.Password;

            //byte[] salt = _cryptoManager.CryptographicRandomNumberGenerator(16);
            //byte[] hash = Argon2Helper.HashPassword(password, salt);

            //var credential = new Credential
            //{
            //    Id = 1,
            //    UserName = "test",
            //    ServiceName = "service test",
            //    EncryptedPassword = hash,
            //};

            //using (var context = new PasswordManagerContext())
            //{
            //    context.Credentials.Add(credential);
            //    await context.SaveChangesAsync();
            //}
            // Close the window after adding.
            this.Close();
        }
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Copy password from PasswordBox to TextBox and make it visible
            VisiblePasswordBox.Text = PasswordBox.Password;
            VisiblePasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Hidden;
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Hide the TextBox and show the PasswordBox again
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
