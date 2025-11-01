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
    public sealed class OrderGridItem
    {
        public int ID_Order { get; set; }
        public string Client_Name { get; set; }
        public System.DateTime Order_Date { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
    }

    public sealed class RequestGridItem
    {
        public int ID_Request { get; set; }
        public string Client_Name { get; set; }
        public string Equipment_Name { get; set; }
        public System.DateTime Created_Date { get; set; }
        public string Problem_Description { get; set; }
        public string Status { get; set; }
    }

    public sealed class WorkGridItem
    {
        public int ID_Work { get; set; }
        public string Client_Name { get; set; }
        public string Equipment_Name { get; set; }
        public System.DateTime Work_Date { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
    }

    public partial class JournalPage : Page
    {
        public JournalPage()
        {
            InitializeComponent();
            TableSelector.SelectedIndex = 0;
            LoadOrders();
        }

        private void LoadOrders()
        {
            List<Orders> orders = App.db.Orders.ToList();
            List<OrderGridItem> items = new List<OrderGridItem>();

            for (int i = 0; i < orders.Count; i++)
            {
                Orders o = orders[i];
                string clientName = o.Clients != null ? o.Clients.Organization_Name : string.Empty;

                OrderGridItem item = new OrderGridItem
                {
                    ID_Order = o.ID_Order,
                    Client_Name = clientName,
                    Order_Date = o.Order_Date,
                    Status = o.Status,
                    Total = o.Total
                };
                items.Add(item);
            }

            MainGrid.ItemsSource = items;
        }

        private void LoadRequests()
        {
            List<Service_Requests> requests = App.db.Service_Requests.ToList();
            List<RequestGridItem> items = new List<RequestGridItem>();

            for (int i = 0; i < requests.Count; i++)
            {
                Service_Requests r = requests[i];
                string clientName = r.Clients != null ? r.Clients.Organization_Name : string.Empty;
                string equipmentName = r.Equipment != null ? r.Equipment.Name : string.Empty;

                RequestGridItem item = new RequestGridItem
                {
                    ID_Request = r.ID_Request,
                    Client_Name = clientName,
                    Equipment_Name = equipmentName,
                    Created_Date = r.Created_Date,
                    Problem_Description = r.Problem_Description,
                    Status = r.Status
                };
                items.Add(item);
            }

            MainGrid.ItemsSource = items;
        }

        private void LoadWorks()
        {
            List<Works> works = App.db.Works.ToList();
            List<WorkGridItem> items = new List<WorkGridItem>();

            for (int i = 0; i < works.Count; i++)
            {
                Works w = works[i];
                string clientName = w.Service_Requests != null && w.Service_Requests.Clients != null
                    ? w.Service_Requests.Clients.Organization_Name
                    : string.Empty;
                string equipmentName = w.Service_Requests != null && w.Service_Requests.Equipment != null
                    ? w.Service_Requests.Equipment.Name
                    : string.Empty;

                WorkGridItem item = new WorkGridItem
                {
                    ID_Work = w.ID_Work,
                    Client_Name = clientName,
                    Equipment_Name = equipmentName,
                    Work_Date = w.Work_Date,
                    Description = w.Description,
                    Cost = w.Cost
                };
                items.Add(item);
            }

            MainGrid.ItemsSource = items;
        }

        private void TableSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TableSelector.SelectedIndex == 0)
            {
                LoadOrders();
            }
            else if (TableSelector.SelectedIndex == 1)
            {
                LoadRequests();
            }
            else if (TableSelector.SelectedIndex == 2)
            {
                LoadWorks();
            }
        }
    }
}