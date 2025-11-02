using PolyPrint.Model;
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
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            Users user = App.db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
