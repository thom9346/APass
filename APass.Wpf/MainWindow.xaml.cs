using APass.Core.Entities;
using APass.Core.Interfaces;
using APass.Core.Services;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace APass.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ICryptographicManager _cryptoManager;
        private IRepository<PasswordEntry> _passwordEntryRepository;
        private IRepository<MasterPassword> _masterPasswordEntryRepository;

        private ObservableCollection<PasswordEntry> _passwordEntries;


        public MainWindow(
            ICryptographicManager cryptoManager, 
            IRepository<PasswordEntry> passwordEntryRepository,
            IRepository<MasterPassword> masterPasswordEntryRepository)
        {
            _cryptoManager = cryptoManager;
            _passwordEntryRepository = passwordEntryRepository;

            InitializeComponent();
            Loaded += MainWindow_Loaded;
            _masterPasswordEntryRepository = masterPasswordEntryRepository;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var dek = SecureSessionService.GetDEK();
                InitializePasswordEntriesList();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("DEK not available. Close the application and start over again.", "Session Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void InitializePasswordEntriesList()
        {
            var passwordEntries = _passwordEntryRepository.GetAll();
            _passwordEntries = new ObservableCollection<PasswordEntry>(passwordEntries);
            PasswordsList.ItemsSource = _passwordEntries;
        }
        private void AddPassword_Click(object sender, RoutedEventArgs e)
        {
            AddPasswordWindow addPasswordWindow = new AddPasswordWindow(_cryptoManager, _passwordEntryRepository, _masterPasswordEntryRepository);
            var result = addPasswordWindow.ShowDialog();

            if (result == true)
            {
                PasswordEntry newPassword = addPasswordWindow.NewPasswordEntry;
                _passwordEntries.Add(newPassword);
            }
        }
        private async void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.CommandParameter is PasswordEntry entry)
                {
                    var encryptedData = Convert.FromBase64String(entry.Password); 
                    var dek = SecureSessionService.GetDEK(); 

                    var decryptedBytes = _cryptoManager.Decrypt(encryptedData, dek);
                    var decryptedPassword = Encoding.UTF8.GetString(decryptedBytes);

                    Clipboard.SetText(decryptedPassword);

                    var originalToolTip = (ToolTip)btn.ToolTip;
                    originalToolTip.Content = "Password copied!";
                    originalToolTip.IsOpen = true;

                    // Wait for a short period before reverting the tooltip content
                    await Task.Delay(2000);

                    originalToolTip.Content = "Copy Password";
                    originalToolTip.IsOpen = false;
                }
            }
            catch (InvalidOperationException ex)
            {
                RedirectUserToLogin("Session has expired, please log in again.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to decrypt password or session expired.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void DeletePassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is PasswordEntry entry)
            {
                var message = $"Are you sure you want to delete all data for {entry.Website}?";
                var result = MessageBox.Show(message, $"Delete Confirmation for ID {entry.ID}", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var canDelete = _passwordEntryRepository.Remove(entry.ID);

                    if(canDelete)
                    {
                        // Remove from ObservableCollection to update the UI
                        _passwordEntries.Remove(entry);
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the password entry. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.CommandParameter is PasswordEntry entry)
            {
                var panel = (StackPanel)checkBox.Parent;
                var passwordTextBlock = (TextBlock)panel.Children[1];

                try
                {
                    var encryptedData = Convert.FromBase64String(entry.Password);
                    var dek = SecureSessionService.GetDEK();
                    var decryptedBytes = _cryptoManager.Decrypt(encryptedData, dek);
                    passwordTextBlock.Text = Encoding.UTF8.GetString(decryptedBytes);
                }
                catch(InvalidOperationException ex)
                {
                    RedirectUserToLogin("Session has expired, please log in again.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to decrypt password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var panel = (StackPanel)checkBox.Parent;
                var passwordTextBlock = (TextBlock)panel.Children[1];
                passwordTextBlock.Text = "********";
            }
        }
        private void RedirectUserToLogin(string message)
        {
            MessageBox.Show(message, "Session Expired", MessageBoxButton.OK, MessageBoxImage.Information);
            LoginWindow loginWindow = new LoginWindow(_cryptoManager, _passwordEntryRepository, _masterPasswordEntryRepository);
            loginWindow.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SecureSessionService.ClearDEK();
            //Application.Current.Shutdown(); // Exit the application
        }
    }
}