using PolyPrint.AppData;
using PolyPrint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint.View.Pages
{
    public partial class AddEquipmentPage : Page
    {
        private static readonly char MetadataSeparator = '|';

        private readonly List<string> _statusOptions = new List<string>
        {
            "Установлено",
            "На обслуживании",
            "Ожидает установки",
            "Снято с обслуживания"
        };

        public AddEquipmentPage()
        {
            InitializeComponent();
            StatusComboBox.ItemsSource = _statusOptions;
            LoadClients();
            LoadEquipment();
        }

        private void LoadClients()
        {
            ClientComboBox.ItemsSource = App.db.Clients
                .OrderBy(c => c.Organization_Name)
                .ToList();
        }

        private void LoadEquipment()
        {
            var statusSet = new HashSet<string>(_statusOptions);

            var items = App.db.Equipment
                .ToList()
                .Select(e =>
                {
                    ParseCondition(e.Condition, out string status, out string supplier, out DateTime? installationDate);
                    if (!string.IsNullOrWhiteSpace(status) && !statusSet.Contains(status))
                    {
                        _statusOptions.Add(status);
                        statusSet.Add(status);
                    }
                    return new
                    {
                        ID = e.ID_Equipment,
                        Name = e.Name,
                        Model = e.Model,
                        Serial = e.Serial_Number,
                        Client = e.Clients != null ? e.Clients.Organization_Name : string.Empty,
                        Supplier = supplier,
                        InstallationDate = installationDate,
                        Status = status
                    };
                })
                .OrderByDescending(e => e.ID)
                .ToList();

            EquipmentGrid.ItemsSource = items;
            StatusComboBox.ItemsSource = null;
            StatusComboBox.ItemsSource = _statusOptions;
        }

        private void SaveEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            string name = StringHelper.Normalize(NameTextBox.Text);
            string model = StringHelper.Normalize(ModelTextBox.Text);
            string serial = StringHelper.Normalize(SerialTextBox.Text);
            string supplier = StringHelper.SanitizeForMetadata(SupplierTextBox.Text, MetadataSeparator);
            string status = StringHelper.Normalize(StatusComboBox.SelectedItem as string ?? StatusComboBox.Text);
            DateTime? installationDate = InstallationDatePicker.SelectedDate;

            if (!ValidationHelper.RequireNotEmpty(new Dictionary<string, string>
                {
                    { "Название", name },
                    { "Модель", model },
                    { "Серийный номер", serial },
                    { "Поставщик", supplier },
                    { "Статус", status }
                }, out string requiredError))
            {
                DialogHelper.ShowWarning(requiredError);
                return;
            }

            if (!ValidationHelper.EnsureDateSelected(installationDate, "Дата установки", out string dateError))
            {
                DialogHelper.ShowWarning(dateError);
                return;
            }

            if (!(ClientComboBox.SelectedItem is Clients selectedClient))
            {
                DialogHelper.ShowWarning("Выберите клиента.");
                return;
            }

            Equipment equipment = new Equipment
            {
                Name = name,
                Model = model,
                Serial_Number = serial,
                ID_Client = selectedClient.ID_Client,
                Condition = ComposeCondition(status, supplier, installationDate)
            };

            if (DbHelper.SaveEntity(equipment, (db, entity) => db.Equipment.Add(entity), out string errorMessage))
            {
                DialogHelper.ShowSuccess("Оборудование сохранено.");
                LoadEquipment();
                ClearForm();
            }
            else
            {
                DialogHelper.ShowError($"Не удалось сохранить оборудование. {errorMessage}");
            }
        }

        private void ClearEquipmentForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            NameTextBox.Text = string.Empty;
            ModelTextBox.Text = string.Empty;
            SerialTextBox.Text = string.Empty;
            SupplierTextBox.Text = string.Empty;
            InstallationDatePicker.SelectedDate = null;
            ClientComboBox.SelectedIndex = -1;
            StatusComboBox.SelectedIndex = -1;
            StatusComboBox.Text = string.Empty;
            NameTextBox.Focus();
        }

        private static string ComposeCondition(string status, string supplier, DateTime? installationDate)
        {
            string sanitizedStatus = StringHelper.SanitizeForMetadata(status, MetadataSeparator);
            string sanitizedSupplier = StringHelper.SanitizeForMetadata(supplier, MetadataSeparator);
            string datePart = installationDate.HasValue ? installationDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            return string.Join(MetadataSeparator.ToString(), new[] { sanitizedStatus, sanitizedSupplier, datePart });
        }

        private static void ParseCondition(string condition, out string status, out string supplier, out DateTime? installationDate)
        {
            status = string.Empty;
            supplier = string.Empty;
            installationDate = null;

            if (string.IsNullOrWhiteSpace(condition))
            {
                return;
            }

            string[] parts = condition.Split(MetadataSeparator);
            if (parts.Length > 0)
            {
                status = parts[0];
            }

            if (parts.Length > 1)
            {
                supplier = parts[1];
            }

            if (parts.Length > 2 && DateTime.TryParse(parts[2], out DateTime parsedDate))
            {
                installationDate = parsedDate;
            }
        }
    }
}
