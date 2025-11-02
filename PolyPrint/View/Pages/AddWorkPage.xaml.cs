using PolyPrint.AppData;
using PolyPrint.Model;
using System;
using System.Collections.Generic;
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
                DialogHelper.ShowWarning("Выберите заявку.");
                return;
            }

            string description = StringHelper.NormalizeMultiline(DescriptionTextBox.Text);
            if (string.IsNullOrWhiteSpace(description))
            {
                DialogHelper.ShowWarning("Опишите выполненные работы.");
                return;
            }

            string costText = StringHelper.Normalize(CostTextBox.Text);
            if (!ValidationHelper.TryParseDecimal(costText, "Стоимость", out decimal cost, out string costError))
            {
                DialogHelper.ShowWarning(costError);
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

            if (DbHelper.SaveEntity(work, (db, entity) => db.Works.Add(entity), out string errorMessage))
            {
                DialogHelper.ShowSuccess("Работа сохранена.");
                LoadWorks();
                ClearForm();
            }
            else
            {
                DialogHelper.ShowError($"Не удалось сохранить работу. {errorMessage}");
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
