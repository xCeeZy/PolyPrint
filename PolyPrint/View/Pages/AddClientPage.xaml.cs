using PolyPrint.AppData;
using PolyPrint.Model;
using System.Collections.Generic;
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
            var items = App.db.Clients
                .ToList()
                .Select(c => new
                {
                    c.Organization_Name,
                    c.Contact_Name,
                    c.Email,
                    Phone = string.IsNullOrWhiteSpace(c.Phone) ? string.Empty : c.Phone
                })
                .OrderBy(c => c.Organization_Name)
                .ToList();

            ClientsGrid.ItemsSource = items;
        }

        private void SaveClientButton_Click(object sender, RoutedEventArgs e)
        {
            string organization = StringHelper.CapitalizeWords(OrganizationTextBox.Text);
            string contact = StringHelper.CapitalizeWords(ContactTextBox.Text);
            string phoneInput = StringHelper.Normalize(PhoneTextBox.Text);
            string email = StringHelper.Normalize(EmailTextBox.Text);

            if (!ValidationHelper.RequireNotEmpty(new Dictionary<string, string>
                {
                    { "Название организации", organization },
                    { "Контактное лицо", contact },
                    { "Телефон", phoneInput }
                }, out string requiredError))
            {
                DialogHelper.ShowWarning(requiredError);
                return;
            }

            if (!ValidationHelper.ValidatePhone(phoneInput, out string phoneError))
            {
                DialogHelper.ShowWarning(phoneError);
                return;
            }

            if (!ValidationHelper.ValidateEmail(email, out string emailError))
            {
                DialogHelper.ShowWarning(emailError);
                return;
            }

            Clients client = new Clients
            {
                Organization_Name = organization,
                Contact_Name = contact,
                Phone = StringHelper.NormalizePhone(phoneInput),
                Email = email
            };

            if (DbHelper.SaveEntity(client, (db, entity) => db.Clients.Add(entity), out string errorMessage))
            {
                DialogHelper.ShowSuccess("Клиент успешно добавлен.");
                LoadClients();
                ClearForm();
            }
            else
            {
                DialogHelper.ShowError($"Не удалось сохранить клиента. {errorMessage}");
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
