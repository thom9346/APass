using APass.Core.Entities;
using APass.Core.Interfaces;
using APass.Core.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace APass.Wpf
{
    /// <summary>
    /// Interaction logic for AddPasswordWindow.xaml
    /// </summary>
    public partial class AddPasswordWindow : Window
    {
        private readonly ICryptographicManager _cryptoManager;
        private IRepository<PasswordEntry> _passwordEntryRepository;
        private IRepository<MasterPassword> _masterPasswordEntryRepository;

        public PasswordEntry NewPasswordEntry { get; private set; }
        public AddPasswordWindow(ICryptographicManager cryptoManager, IRepository<PasswordEntry> passwordEntryRepository, IRepository<MasterPassword> masterPasswordEntryRepository)
        {
            _cryptoManager = cryptoManager;
            _passwordEntryRepository = passwordEntryRepository;
            InitializeComponent();
            _masterPasswordEntryRepository = masterPasswordEntryRepository;
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var website = WebsiteTextBox.Text;
                var username = UsernameTextBox.Text;
                var plaintextPassword = PasswordBox.Password;

                var dek = SecureSessionService.GetDEK();

                var plaintextBytes = Encoding.UTF8.GetBytes(plaintextPassword);
                var encryptedPasswordWithIV = _cryptoManager.Encrypt(plaintextBytes, dek);

                var encryptedPasswordBase64 = Convert.ToBase64String(encryptedPasswordWithIV);

                NewPasswordEntry = new PasswordEntry
                {
                    Website = website,
                    Username = username,
                    Password = encryptedPasswordBase64
                };

                _passwordEntryRepository.Add(NewPasswordEntry);
                this.DialogResult = true;
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                OpenLoginWindow("Session has expired, please log in again.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OpenLoginWindow(string message)
        {
            MessageBox.Show(message, "Session Expired", MessageBoxButton.OK, MessageBoxImage.Information);
            LoginWindow loginWindow = new LoginWindow(_cryptoManager, _passwordEntryRepository, _masterPasswordEntryRepository);
            loginWindow.Show();
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
            var rdmLength = rdm.Next(16, 32 + 1);

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
