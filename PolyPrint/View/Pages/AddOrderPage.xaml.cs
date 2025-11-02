using PolyPrint.AppData;
using PolyPrint.Model;
using System;
using System.Collections.Generic;
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
                DialogHelper.ShowWarning("Выберите клиента.");
                return;
            }

            if (!ValidationHelper.EnsureDateSelected(OrderDatePicker.SelectedDate, "Дата заказа", out string dateError))
            {
                DialogHelper.ShowWarning(dateError);
                return;
            }

            string status = StringHelper.Normalize(StatusComboBox.SelectedItem as string ?? StatusComboBox.Text);
            if (string.IsNullOrWhiteSpace(status))
            {
                DialogHelper.ShowWarning("Выберите статус заказа.");
                return;
            }

            string totalText = StringHelper.Normalize(TotalTextBox.Text);
            if (!ValidationHelper.TryParseDecimal(totalText, "Сумма", out decimal total, out string totalError))
            {
                DialogHelper.ShowWarning(totalError);
                return;
            }

            Orders order = new Orders
            {
                ID_Client = selectedClient.ID_Client,
                Order_Date = OrderDatePicker.SelectedDate.Value,
                Status = status,
                Total = total
            };

            if (DbHelper.SaveEntity(order, (db, entity) => db.Orders.Add(entity), out string errorMessage))
            {
                DialogHelper.ShowSuccess("Заказ успешно сохранен.");
                LoadOrders();
                ClearForm();
            }
            else
            {
                DialogHelper.ShowError($"Не удалось сохранить заказ. {errorMessage}");
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
            StatusComboBox.Text = string.Empty;
            TotalTextBox.Text = string.Empty;
        }
    }
}
