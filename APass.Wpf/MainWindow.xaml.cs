using APass.Core.Entities;
using APass.Core.Interfaces;
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
        private ObservableCollection<PasswordEntry> _passwordEntries;


        public MainWindow(ICryptographicManager cryptoManager)
        {
            _cryptoManager = cryptoManager;
            InitializeComponent();
            InitializePasswordEntriesList();

        }
        private void InitializePasswordEntriesList()
        {
            // Initialize the ObservableCollection
            _passwordEntries = new ObservableCollection<PasswordEntry>();
            PasswordsList.ItemsSource = _passwordEntries;
        }
        private void AddPassword_Click(object sender, RoutedEventArgs e)
        {
            AddPasswordWindow addPasswordWindow = new AddPasswordWindow(_cryptoManager);
            var result = addPasswordWindow.ShowDialog(); // Show the Add Password window as a dialog

            if (result == true)
            {
                PasswordEntry newPassword = addPasswordWindow.NewPasswordEntry;
                _passwordEntries.Add(newPassword);
            }
        }
    }
}