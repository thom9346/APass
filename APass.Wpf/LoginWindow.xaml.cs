using APass.Core.Entities;
using APass.Core.Interfaces;
using APass.Core.Services;
using System.Windows;
using System.Windows.Controls;

namespace APass.Wpf
{
    public partial class LoginWindow : Window
    {
        private readonly ICryptographicManager _cryptoManager;
        private IRepository<PasswordEntry> _passwordEntryRepository;
        private IRepository<MasterPassword> _masterPasswordEntryRepository;

        public LoginWindow(
            ICryptographicManager cryptoManager,
            IRepository<PasswordEntry> passwordEntryRepository,
            IRepository<MasterPassword> masterPasswordEntryRepository)
        {
            _cryptoManager = cryptoManager;
            _passwordEntryRepository = passwordEntryRepository;
            InitializeComponent();
            _masterPasswordEntryRepository = masterPasswordEntryRepository;
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var masterPassword = PasswordBox.Password;

            bool isValid = CheckMasterPassword(masterPassword);

            if (isValid)
            {
                MainWindow mainWindow = new MainWindow(_cryptoManager, _passwordEntryRepository);
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
            // Retrieve the stored MasterPassword entity, which contains your salt and encrypted DEK
            MasterPassword masterPasswordEntity = _masterPasswordEntryRepository.Get(1);
            if (masterPasswordEntity == null)
            {
                MessageBox.Show("Master Password setup is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            byte[] salt = masterPasswordEntity.Salt;
            byte[] encryptedDEK = masterPasswordEntity.EncryptedDEK;

            // Re-derive the KEK (Key Encryption Key) from the entered password and the stored salt
            byte[] derivedKey = _cryptoManager.DeriveKey(password, salt, 10000, 32);

            try
            {
                // Attempt to decrypt the DEK with the derived KEK
                // This step is to verify that the provided master password is correct
                // If the password is incorrect, decryption will fail and throw an exception
                byte[] dek = _cryptoManager.Decrypt(encryptedDEK, derivedKey);
                SecureSessionService.StoreDEK(dek);
                return true;
            }
            catch
            {
                // If decryption fails, it means the entered password is incorrect
                return false;
            }
        }
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignupWindow signUpWindow = new SignupWindow(_cryptoManager, _masterPasswordEntryRepository);
            signUpWindow.ShowDialog();
        }
    }
}
