﻿using APass.Core.Entities;
using APass.Core.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace APass.Wpf
{
    /// <summary>
    /// Interaction logic for SignupWindow.xaml
    /// </summary>
    public partial class SignupWindow : Window
    {
        private readonly ICryptographicManager _cryptoManager;
        private IRepository<MasterPassword> _masterPasswordRepository;

        public SignupWindow(ICryptographicManager cryptoManager, IRepository<MasterPassword> masterPasswordRepository)
        {
            _cryptoManager = cryptoManager;
            _masterPasswordRepository = masterPasswordRepository;
            InitializeComponent();
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string masterPassword = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            var strength = CalculatePasswordStrength(masterPassword);
            bool hasUpperCase = masterPassword.Any(char.IsUpper);
            bool hasNumber = masterPassword.Any(char.IsDigit);
            bool hasSpecial = masterPassword.Any(c => !char.IsLetterOrDigit(c));

            if (string.IsNullOrWhiteSpace(masterPassword) || masterPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match or are empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (strength < 65)
            {
                MessageBox.Show("That password is not strong enough. Please try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            else if (!hasUpperCase || !hasNumber || !hasSpecial)
            {
                string errorMessage = "Password must include at least: \n- 1 uppercase letter\n- 1 number\n- 1 special character";
                MessageBox.Show(errorMessage, "Password Requirements Not Met", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                byte[] salt = _cryptoManager.CryptographicRandomNumberGenerator(16);
                byte[] derivedKey = _cryptoManager.DeriveKey(masterPassword, salt, 10000, 32);
                byte[] databaseEncryptionKey = _cryptoManager.CryptographicRandomNumberGenerator(32); // DEK for AES-256
                byte[] encryptedDEK = _cryptoManager.Encrypt(databaseEncryptionKey, derivedKey);

                var masterKey = new MasterPassword
                {
                    EncryptedDEK = encryptedDEK,
                    Salt = salt,
                };

                _masterPasswordRepository.Add(masterKey);

                MessageBox.Show("Master password set up successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password = PasswordBox.Password;
            var strength = CalculatePasswordStrength(password);

            PasswordStrengthBar.Value = strength;
            if (strength < 40)
            {
                PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Red);
                PasswordStrengthText.Text = "Weak!";
            }
            else if (strength < 70)
            {
                PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Yellow);
                PasswordStrengthText.Text = "Moderate";
            }
            else
            {
                PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Green);
                PasswordStrengthText.Text = "Great!";
            }

            //Check for password requirements
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasNumber = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            string requirementsMessage = "";
            if (!hasUpperCase) requirementsMessage += "Requires at least 1 uppercase letter. ";
            if (!hasNumber) requirementsMessage += "Requires at least 1 number. ";
            if (!hasSpecial) requirementsMessage += "Requires at least 1 special character.";

            PasswordRequirementsText.Text = requirementsMessage;
        }


        //TODO: Implement some sort of look-up table... Something like "P@sSw0Rd" should be considered weak.
        private int CalculatePasswordStrength(string password)
        {
            int score = 0;

            //Increase score for password length
            score += password.Length * 2;
            int upperCase = 0, lowerCase = 0, numbers = 0, special = 0;

            //Check for presence of uppercase letters, lowercase letters, numbers, and special characters
            foreach (char c in password)
            {
                if (char.IsUpper(c)) upperCase++;
                if (char.IsLower(c)) lowerCase++;
                if (char.IsDigit(c)) numbers++;
                if (!char.IsLetterOrDigit(c)) special++;
            }

            if (upperCase > 0) score += 5;
            if (lowerCase > 0) score += 5;
            if (numbers > 0) score += 5;
            if (special > 0) score += 5;

            //additional bonuses
            if (upperCase > 0 && lowerCase > 0) score += 5; //mixed case
            if (numbers > 0 && (upperCase > 0 || lowerCase > 0)) score += 5; //Letters and numbers
            if (special > 0 && (upperCase > 0 || lowerCase > 0)) score += 5; //letters and special characters
            if (special > 0 && numbers > 0) score += 5; //Numbers and special characters
            if (special > 0 && numbers > 0 && (upperCase > 0 || lowerCase > 0)) score += 5; //Mixed types

            //score capped at 100 
            score = Math.Min(score, 100);

            return score;
        }
    }
}
