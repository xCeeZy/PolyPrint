using PolyPrint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint.View.Pages
{
    public partial class AddServiceRequestPage : Page
    {
        private readonly List<string> _statusOptions = new List<string>
        {
            "Новая",
            "В работе",
            "Завершена",
            "Отменена"
        };

        public AddServiceRequestPage()
        {
            InitializeComponent();
            StatusComboBox.ItemsSource = _statusOptions;
            CreatedDatePicker.SelectedDate = DateTime.Today;
            LoadClients();
            LoadMasters();
            LoadEquipment();
            LoadRequests();
        }

        private void LoadClients()
        {
            ClientComboBox.ItemsSource = App.db.Clients
                .OrderBy(c => c.Organization_Name)
                .ToList();
        }

        private void LoadMasters()
        {
            MasterComboBox.ItemsSource = App.db.Users
                .OrderBy(u => u.Login)
                .ToList();
        }

        private void LoadEquipment(int? clientId = null)
        {
            var equipmentQuery = App.db.Equipment.AsQueryable();

            if (clientId.HasValue)
            {
                equipmentQuery = equipmentQuery.Where(eq => eq.ID_Client == clientId.Value);
            }

            EquipmentComboBox.ItemsSource = equipmentQuery
                .OrderBy(eq => eq.Name)
                .ToList();
        }

        private void LoadRequests()
        {
            var items = App.db.Service_Requests
                .ToList()
                .Select(r => new
                {
                    ID = r.ID_Request,
                    Client = r.Clients != null ? r.Clients.Organization_Name : string.Empty,
                    Equipment = r.Equipment != null ? r.Equipment.Name : string.Empty,
                    Created = r.Created_Date,
                    Status = r.Status,
                    Master = r.Users != null ? r.Users.Login : string.Empty
                })
                .OrderByDescending(r => r.ID)
                .ToList();

            RequestsGrid.ItemsSource = items;
        }

        private void SaveRequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ClientComboBox.SelectedItem is Clients client))
            {
                MessageBox.Show("Выберите клиента", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string status = StatusComboBox.SelectedItem as string ?? StatusComboBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show("Укажите статус заявки", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string description = ProblemDescriptionTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Опишите проблему", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DateTime createdDate = CreatedDatePicker.SelectedDate ?? DateTime.Today;
            Equipment selectedEquipment = EquipmentComboBox.SelectedItem as Equipment;
            Users selectedMaster = MasterComboBox.SelectedItem as Users;

            Service_Requests request = new Service_Requests
            {
                ID_Client = client.ID_Client,
                ID_Equipment = selectedEquipment?.ID_Equipment,
                Created_Date = createdDate,
                Problem_Description = description,
                Status = status,
                ID_Master = selectedMaster?.ID_User
            };

            try
            {
                App.db.Service_Requests.Add(request);
                App.db.SaveChanges();
                MessageBox.Show("Заявка сохранена", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadRequests();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить заявку. {ex.Message}", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearRequestForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ClientComboBox.SelectedIndex = -1;
            EquipmentComboBox.ItemsSource = null;
            StatusComboBox.SelectedIndex = -1;
            MasterComboBox.SelectedIndex = -1;
            ProblemDescriptionTextBox.Text = string.Empty;
            CreatedDatePicker.SelectedDate = DateTime.Today;
            LoadEquipment();
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedItem is Clients client)
            {
                LoadEquipment(client.ID_Client);
            }
            else
            {
                LoadEquipment();
            }
        }
    }
}
