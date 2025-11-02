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
        private readonly List<string> _conditions = new List<string>
        {
            "Новое",
            "В эксплуатации",
            "Требует ремонта",
            "Списано"
        };

        public AddEquipmentPage()
        {
            InitializeComponent();
            ConditionComboBox.ItemsSource = _conditions;
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
            var items = App.db.Equipment
                .ToList()
                .Select(e => new
                {
                    ID = e.ID_Equipment,
                    Name = e.Name,
                    Model = e.Model,
                    Serial = e.Serial_Number,
                    Client = e.Clients != null ? e.Clients.Organization_Name : string.Empty,
                    Condition = e.Condition
                })
                .OrderByDescending(e => e.ID)
                .ToList();

            EquipmentGrid.ItemsSource = items;
        }

        private void SaveEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название оборудования", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string model = ModelTextBox.Text?.Trim();
            string serial = SerialTextBox.Text?.Trim();
            string condition = ConditionComboBox.Text?.Trim();

            Clients selectedClient = ClientComboBox.SelectedItem as Clients;

            Equipment equipment = new Equipment
            {
                Name = name,
                Model = model,
                Serial_Number = serial,
                ID_Client = selectedClient?.ID_Client,
                Condition = condition
            };

            try
            {
                App.db.Equipment.Add(equipment);
                App.db.SaveChanges();
                MessageBox.Show("Оборудование сохранено", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadEquipment();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить оборудование. {ex.Message}", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Error);
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
            ConditionComboBox.Text = string.Empty;
            ClientComboBox.SelectedIndex = -1;
            NameTextBox.Focus();
        }
    }
}
