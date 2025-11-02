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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolyPrint.View.Pages
{
    public partial class AddClientPage : Page
    {
        public AddClientPage()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
            ClearButton.Click += ClearButton_Click;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string organizationName = OrganizationNameBox.Text.Trim();
            string contactName = ContactNameBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox.Text.Trim();

            if (organizationName.Length == 0 || phone.Length == 0)
            {
                MessageBox.Show("Заполните обязательные поля: название и телефон");
                return;
            }

            Clients client = new Clients();
            client.ID_Client = App.db.Clients.Any() ? App.db.Clients.Max(c => c.ID_Client) + 1 : 1;
            client.Organization_Name = organizationName;
            client.Contact_Name = contactName;
            client.Phone = phone;
            client.Email = email;

            App.db.Clients.Add(client);
            App.db.SaveChanges();

            MessageBox.Show("Клиент успешно добавлен");

            ClearFields();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            OrganizationNameBox.Text = string.Empty;
            ContactNameBox.Text = string.Empty;
            PhoneBox.Text = string.Empty;
            EmailBox.Text = string.Empty;
        }
    }
}