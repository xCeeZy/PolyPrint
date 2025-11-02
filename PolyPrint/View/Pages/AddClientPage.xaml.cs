using PolyPrint.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint.View.Pages
{
    public partial class AddClientPage : Page
    {
        public AddClientPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            ClientsGrid.ItemsSource = App.db.Clients.ToList();
        }

        private void SaveClientButton_Click(object sender, RoutedEventArgs e)
        {
            string organization = OrganizationTextBox.Text?.Trim() ?? string.Empty;
            string contact = ContactTextBox.Text?.Trim() ?? string.Empty;
            string phone = PhoneTextBox.Text?.Trim() ?? string.Empty;
            string email = EmailTextBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(organization))
            {
                MessageBox.Show("Введите название организации", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Clients client = new Clients
            {
                Organization_Name = organization,
                Contact_Name = contact,
                Phone = phone,
                Email = email
            };

            try
            {
                App.db.Clients.Add(client);
                App.db.SaveChanges();
                MessageBox.Show("Клиент успешно добавлен", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadClients();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить клиента. {ex.Message}", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearClientForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            OrganizationTextBox.Text = string.Empty;
            ContactTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            OrganizationTextBox.Focus();
        }
    }
}
