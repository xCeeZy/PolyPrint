using PolyPrint.AppData;
using PolyPrint.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PolyPrint.View.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += (_, __) => LoginTextBox.Focus();
        }

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

            Users user = App.db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                DialogHelper.ShowWarning("Неверный логин или пароль.");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
