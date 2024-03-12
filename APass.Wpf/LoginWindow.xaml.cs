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
            _masterPasswordEntryRepository = masterPasswordEntryRepository;

            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var masterPassword = PasswordBox.Password;

            bool isValid = CheckMasterPassword(masterPassword);

            if (isValid)
            {
                //check if MainWindow is already open
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow == null)
                {
                    mainWindow = new MainWindow(_cryptoManager, _passwordEntryRepository, _masterPasswordEntryRepository);
                    mainWindow.Show();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect Master Password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CheckMasterPassword(string password)
        {
            MasterPassword masterPasswordEntity = _masterPasswordEntryRepository.Get(1);
            if (masterPasswordEntity == null)
            {
                MessageBox.Show("Master Password setup is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            byte[] salt = masterPasswordEntity.Salt;
            byte[] encryptedDEK = masterPasswordEntity.EncryptedDEK;

            //re-derive the KEK (Key Encryption Key) from the entered password and the stored salt
            byte[] derivedKey = _cryptoManager.DeriveKey(password, salt, 10000, 32);

            try
            {
                byte[] dek = _cryptoManager.Decrypt(encryptedDEK, derivedKey);
                SecureSessionService.StoreDEK(dek);
                return true;
            }
            catch
            {
                //if decryption fails, it means the entered password is incorrect
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
