using PolyPrint.Model;
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

namespace PolyPrint.View.Windows
{
    public partial class LoginWindow : Window
    {
        #region Инициализация

        public LoginWindow()
        {
            InitializeComponent();

            LoginButton.Click += LoginButton_Click;
            PasswordBox.KeyDown += PasswordBox_KeyDown;
            LoginTextBox.KeyDown += LoginTextBox_KeyDown;

            LoginTextBox.Focus();
        }

        #endregion

        #region Обработка ввода

        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox.Focus();
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        #endregion

        #region Авторизация

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void PerformLogin()
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Users user = App.db.Users
                .FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.",
                    "Ошибка авторизации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                PasswordBox.Clear();
                PasswordBox.Focus();
                return;
            }

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Close();
        }

        #endregion
    }
}