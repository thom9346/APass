using APass.Core.Entities;
using APass.Core.Interfaces;
using APass.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APass.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ICryptographicManager _cryptoManager;
        private IRepository<PasswordEntry> _passwordEntryRepository;
        private ObservableCollection<PasswordEntry> _passwordEntries;
        private readonly PasswordManagerContext _dbContext;

        public MainWindow(
            ICryptographicManager cryptoManager, 
            PasswordManagerContext dbContext,
            IRepository<PasswordEntry> passwordEntryRepository)
        {
            _cryptoManager = cryptoManager;
            _dbContext = dbContext;
            _passwordEntryRepository = passwordEntryRepository;

            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializePasswordEntriesListAsync(); 
        }
        private async Task InitializePasswordEntriesListAsync()
        {
            var passwordEntries = await _dbContext.PasswordEntries.ToListAsync();
            _passwordEntries = new ObservableCollection<PasswordEntry>(passwordEntries);
            PasswordsList.ItemsSource = _passwordEntries;
        }
        private void AddPassword_Click(object sender, RoutedEventArgs e)
        {
            AddPasswordWindow addPasswordWindow = new AddPasswordWindow(_cryptoManager, _dbContext);
            var result = addPasswordWindow.ShowDialog();

            if (result == true)
            {
                PasswordEntry newPassword = addPasswordWindow.NewPasswordEntry;
                _passwordEntries.Add(newPassword);
            }
        }
        private async void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string password)
            {
                Clipboard.SetText(password);
                var originalToolTip = (ToolTip)btn.ToolTip;
                originalToolTip.Content = "Password copied!";
                originalToolTip.IsOpen = true;

                // Wait for a short period before reverting the tooltip content
                await Task.Delay(2000);

                originalToolTip.Content = "Copy Password";
                originalToolTip.IsOpen = false;
            }
        }
        private async void DeletePassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is PasswordEntry entry)
            {
                var message = $"Are you sure you want to delete all data for {entry.Website}?";
                var result = MessageBox.Show(message, "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
    }
}