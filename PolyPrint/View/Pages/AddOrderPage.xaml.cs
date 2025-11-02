using PolyPrint.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint.View.Pages
{
    public partial class AddOrderPage : Page
    {
        private readonly List<string> _statusOptions = new List<string>
        {
            "Новый",
            "В работе",
            "Завершен",
            "Отменен"
        };

        public AddOrderPage()
        {
            InitializeComponent();
            LoadClients();
            LoadOrders();
            StatusComboBox.ItemsSource = _statusOptions;
            OrderDatePicker.SelectedDate = DateTime.Today;
        }

        private void LoadClients()
        {
            ClientComboBox.ItemsSource = App.db.Clients
                .OrderBy(c => c.Organization_Name)
                .ToList();
        }

        private void LoadOrders()
        {
            var items = App.db.Orders
                .ToList()
                .Select(o => new
                {
                    ID = o.ID_Order,
                    Client = o.Clients != null ? o.Clients.Organization_Name : string.Empty,
                    Date = o.Order_Date,
                    Status = o.Status,
                    Total = o.Total
                })
                .OrderByDescending(o => o.ID)
                .ToList();

            OrdersGrid.ItemsSource = items;
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ClientComboBox.SelectedItem is Clients selectedClient))
            {
                MessageBox.Show("Выберите клиента", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!(OrderDatePicker.SelectedDate is DateTime orderDate))
            {
                MessageBox.Show("Укажите дату заказа", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string status = StatusComboBox.SelectedItem as string ?? StatusComboBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show("Выберите статус заказа", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string totalText = TotalTextBox.Text?.Trim() ?? string.Empty;
            if (!decimal.TryParse(totalText, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal total) &&
                !decimal.TryParse(totalText, NumberStyles.Number, CultureInfo.InvariantCulture, out total))
            {
                MessageBox.Show("Некорректное значение суммы", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Orders order = new Orders
            {
                ID_Client = selectedClient.ID_Client,
                Order_Date = orderDate,
                Status = status,
                Total = total
            };

            try
            {
                App.db.Orders.Add(order);
                App.db.SaveChanges();
                MessageBox.Show("Заказ успешно сохранен", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadOrders();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить заказ. {ex.Message}", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearOrderForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ClientComboBox.SelectedIndex = -1;
            OrderDatePicker.SelectedDate = DateTime.Today;
            StatusComboBox.SelectedIndex = -1;
            TotalTextBox.Text = string.Empty;
        }
    }
}
