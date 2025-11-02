using PolyPrint.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint.View.Pages
{
    public partial class AddWorkPage : Page
    {
        private sealed class RequestOption
        {
            public Service_Requests Request { get; set; }
            public string Display { get; set; }
        }

        private List<RequestOption> _requestOptions = new List<RequestOption>();

        public AddWorkPage()
        {
            InitializeComponent();
            LoadRequestOptions();
            LoadWorks();
            WorkDatePicker.SelectedDate = DateTime.Today;
        }

        private void LoadRequestOptions()
        {
            _requestOptions = App.db.Service_Requests
                .ToList()
                .Select(r => new RequestOption
                {
                    Request = r,
                    Display = $"#{r.ID_Request} · {r.Clients?.Organization_Name ?? "Без клиента"}"
                })
                .OrderByDescending(r => r.Request.ID_Request)
                .ToList();

            RequestComboBox.ItemsSource = _requestOptions;
        }

        private void LoadWorks()
        {
            var items = App.db.Works
                .ToList()
                .Select(w => new
                {
                    ID = w.ID_Work,
                    Request = $"#{w.ID_Request}",
                    Client = w.Service_Requests?.Clients?.Organization_Name ?? string.Empty,
                    Description = w.Description,
                    Date = w.Work_Date,
                    Cost = w.Cost
                })
                .OrderByDescending(w => w.ID)
                .ToList();

            WorksGrid.ItemsSource = items;
        }

        private void SaveWorkButton_Click(object sender, RoutedEventArgs e)
        {
            Service_Requests selectedRequest = RequestComboBox.SelectedValue as Service_Requests;
            if (selectedRequest == null && RequestComboBox.SelectedItem is RequestOption option)
            {
                selectedRequest = option.Request;
            }
            if (selectedRequest == null)
            {
                MessageBox.Show("Выберите заявку", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string description = DescriptionTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Опишите выполненные работы", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string costText = CostTextBox.Text?.Trim() ?? string.Empty;
            if (!decimal.TryParse(costText, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal cost) &&
                !decimal.TryParse(costText, NumberStyles.Number, CultureInfo.InvariantCulture, out cost))
            {
                MessageBox.Show("Стоимость указана некорректно", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DateTime workDate = WorkDatePicker.SelectedDate ?? DateTime.Today;

            Works work = new Works
            {
                ID_Request = selectedRequest.ID_Request,
                Description = description,
                Cost = cost,
                Work_Date = workDate
            };

            try
            {
                App.db.Works.Add(work);
                App.db.SaveChanges();
                MessageBox.Show("Работа сохранена", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadWorks();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить работу. {ex.Message}", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearWorkForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            RequestComboBox.SelectedIndex = -1;
            DescriptionTextBox.Text = string.Empty;
            CostTextBox.Text = string.Empty;
            WorkDatePicker.SelectedDate = DateTime.Today;
            LoadRequestOptions();
        }
    }
}
