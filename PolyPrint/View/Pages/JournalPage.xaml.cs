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
    public partial class JournalPage : Page
    {
        #region Инициализация

        public JournalPage()
        {
            InitializeComponent();

            TableSelector.SelectionChanged += TableSelector_SelectionChanged;
            SearchBox.TextChanged += SearchBox_TextChanged;
            RefreshButton.Click += RefreshButton_Click;

            LoadOrders();
        }

        #endregion

        #region Загрузка данных

        private void TableSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TableSelector.SelectedIndex < 0)
            {
                return;
            }

            SearchBox.Clear();

            switch (TableSelector.SelectedIndex)
            {
                case 0:
                    LoadOrders();
                    break;
                case 1:
                    LoadServiceRequests();
                    break;
                case 2:
                    LoadWorks();
                    break;
                case 3:
                    LoadEquipment();
                    break;
                case 4:
                    LoadClients();
                    break;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            TableSelector_SelectionChanged(null, null);
        }

        #endregion

        #region Загрузка заказов

        private void LoadOrders()
        {
            try
            {
                List<OrderGridItem> orders = App.db.Orders
                    .ToList()
                    .Select(o => new OrderGridItem
                    {
                        ID = o.ID_Order,
                        Client = o.Clients.Organization_Name,
                        OrderDate = o.Order_Date.ToString("dd.MM.yyyy"),
                        Status = o.Status,
                        Total = o.Total.ToString("N2") + " ₽"
                    })
                    .ToList();

                MainGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Загрузка сервисных заявок

        private void LoadServiceRequests()
        {
            try
            {
                List<ServiceRequestGridItem> requests = App.db.Service_Requests
                    .ToList()
                    .Select(sr => new ServiceRequestGridItem
                    {
                        ID = sr.ID_Request,
                        Client = sr.Clients.Organization_Name,
                        Equipment = sr.Equipment?.Name ?? "—",
                        CreatedDate = sr.Created_Date.ToString("dd.MM.yyyy"),
                        Problem = sr.Problem_Description,
                        Status = sr.Status,
                        Master = sr.Users?.Login ?? "—"
                    })
                    .ToList();

                MainGrid.ItemsSource = requests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Загрузка выполненных работ

        private void LoadWorks()
        {
            try
            {
                List<WorkGridItem> works = App.db.Works
                    .ToList()
                    .Select(w => new WorkGridItem
                    {
                        ID = w.ID_Work,
                        RequestID = w.ID_Request,
                        Description = w.Description,
                        Cost = w.Cost.ToString("N2") + " ₽",
                        WorkDate = w.Work_Date.ToString("dd.MM.yyyy")
                    })
                    .ToList();

                MainGrid.ItemsSource = works;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки работ:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Загрузка оборудования

        private void LoadEquipment()
        {
            try
            {
                List<EquipmentGridItem> equipment = App.db.Equipment
                    .ToList()
                    .Select(eq => new EquipmentGridItem
                    {
                        ID = eq.ID_Equipment,
                        Name = eq.Name,
                        Model = eq.Model,
                        SerialNumber = eq.Serial_Number,
                        Client = eq.Clients?.Organization_Name ?? "—",
                        Condition = eq.Condition
                    })
                    .ToList();

                MainGrid.ItemsSource = equipment;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки оборудования:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Загрузка клиентов

        private void LoadClients()
        {
            try
            {
                List<ClientGridItem> clients = App.db.Clients
                    .ToList()
                    .Select(c => new ClientGridItem
                    {
                        ID = c.ID_Client,
                        OrganizationName = c.Organization_Name,
                        ContactName = c.Contact_Name,
                        Phone = c.Phone,
                        Email = c.Email
                    })
                    .ToList();

                MainGrid.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Поиск

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                TableSelector_SelectionChanged(null, null);
                return;
            }

            if (MainGrid.ItemsSource == null)
            {
                return;
            }

            switch (TableSelector.SelectedIndex)
            {
                case 0:
                    FilterOrders(searchText);
                    break;
                case 1:
                    FilterServiceRequests(searchText);
                    break;
                case 2:
                    FilterWorks(searchText);
                    break;
                case 3:
                    FilterEquipment(searchText);
                    break;
                case 4:
                    FilterClients(searchText);
                    break;
            }
        }

        private void FilterOrders(string searchText)
        {
            List<OrderGridItem> filtered = App.db.Orders
                .ToList()
                .Where(o =>
                    o.Clients.Organization_Name.ToLower().Contains(searchText) ||
                    o.Status.ToLower().Contains(searchText) ||
                    o.ID_Order.ToString().Contains(searchText))
                .Select(o => new OrderGridItem
                {
                    ID = o.ID_Order,
                    Client = o.Clients.Organization_Name,
                    OrderDate = o.Order_Date.ToString("dd.MM.yyyy"),
                    Status = o.Status,
                    Total = o.Total.ToString("N2") + " ₽"
                })
                .ToList();

            MainGrid.ItemsSource = filtered;
        }

        private void FilterServiceRequests(string searchText)
        {
            List<ServiceRequestGridItem> filtered = App.db.Service_Requests
                .ToList()
                .Where(sr =>
                    sr.Clients.Organization_Name.ToLower().Contains(searchText) ||
                    sr.Problem_Description.ToLower().Contains(searchText) ||
                    sr.Status.ToLower().Contains(searchText) ||
                    (sr.Equipment?.Name.ToLower().Contains(searchText) ?? false))
                .Select(sr => new ServiceRequestGridItem
                {
                    ID = sr.ID_Request,
                    Client = sr.Clients.Organization_Name,
                    Equipment = sr.Equipment?.Name ?? "—",
                    CreatedDate = sr.Created_Date.ToString("dd.MM.yyyy"),
                    Problem = sr.Problem_Description,
                    Status = sr.Status,
                    Master = sr.Users?.Login ?? "—"
                })
                .ToList();

            MainGrid.ItemsSource = filtered;
        }

        private void FilterWorks(string searchText)
        {
            List<WorkGridItem> filtered = App.db.Works
                .ToList()
                .Where(w =>
                    w.Description.ToLower().Contains(searchText) ||
                    w.ID_Work.ToString().Contains(searchText))
                .Select(w => new WorkGridItem
                {
                    ID = w.ID_Work,
                    RequestID = w.ID_Request,
                    Description = w.Description,
                    Cost = w.Cost.ToString("N2") + " ₽",
                    WorkDate = w.Work_Date.ToString("dd.MM.yyyy")
                })
                .ToList();

            MainGrid.ItemsSource = filtered;
        }

        private void FilterEquipment(string searchText)
        {
            List<EquipmentGridItem> filtered = App.db.Equipment
                .ToList()
                .Where(eq =>
                    eq.Name.ToLower().Contains(searchText) ||
                    eq.Model.ToLower().Contains(searchText) ||
                    eq.Serial_Number.ToLower().Contains(searchText) ||
                    (eq.Clients?.Organization_Name.ToLower().Contains(searchText) ?? false))
                .Select(eq => new EquipmentGridItem
                {
                    ID = eq.ID_Equipment,
                    Name = eq.Name,
                    Model = eq.Model,
                    SerialNumber = eq.Serial_Number,
                    Client = eq.Clients?.Organization_Name ?? "—",
                    Condition = eq.Condition
                })
                .ToList();

            MainGrid.ItemsSource = filtered;
        }

        private void FilterClients(string searchText)
        {
            List<ClientGridItem> filtered = App.db.Clients
                .ToList()
                .Where(c =>
                    c.Organization_Name.ToLower().Contains(searchText) ||
                    c.Contact_Name.ToLower().Contains(searchText) ||
                    c.Phone.ToLower().Contains(searchText) ||
                    c.Email.ToLower().Contains(searchText))
                .Select(c => new ClientGridItem
                {
                    ID = c.ID_Client,
                    OrganizationName = c.Organization_Name,
                    ContactName = c.Contact_Name,
                    Phone = c.Phone,
                    Email = c.Email
                })
                .ToList();

            MainGrid.ItemsSource = filtered;
        }

        #endregion
    }

    #region Классы для отображения данных

    public class OrderGridItem
    {
        public int ID { get; set; }
        public string Client { get; set; }
        public string OrderDate { get; set; }
        public string Status { get; set; }
        public string Total { get; set; }
    }

    public class ServiceRequestGridItem
    {
        public int ID { get; set; }
        public string Client { get; set; }
        public string Equipment { get; set; }
        public string CreatedDate { get; set; }
        public string Problem { get; set; }
        public string Status { get; set; }
        public string Master { get; set; }
    }

    public class WorkGridItem
    {
        public int ID { get; set; }
        public int RequestID { get; set; }
        public string Description { get; set; }
        public string Cost { get; set; }
        public string WorkDate { get; set; }
    }

    public class EquipmentGridItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Client { get; set; }
        public string Condition { get; set; }
    }

    public class ClientGridItem
    {
        public int ID { get; set; }
        public string OrganizationName { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}

    #endregion