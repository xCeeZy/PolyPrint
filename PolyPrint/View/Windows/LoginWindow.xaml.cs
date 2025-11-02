using PolyPrint.AppData;
using PolyPrint.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PolyPrint.View.Windows
{
    public partial class LoginWindow : Window
    {
        #region Инициализация

        public LoginWindow()
        {
            InitializeComponent();
            Loaded += (_, __) => LoginTextBox.Focus();
        }

        #endregion

        #region Авторизация

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = StringHelper.Normalize(LoginTextBox.Text);
            string password = PasswordBox.Password ?? string.Empty;

            if (!ValidationHelper.RequireNotEmpty(new Dictionary<string, string>
                {
                    { "Логин", login },
                    { "Пароль", password }
                }, out string requiredError))
            {
                DialogHelper.ShowWarning(requiredError);
                return;
            }
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
                DialogHelper.ShowWarning("Неверный логин или пароль.");
            }

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}