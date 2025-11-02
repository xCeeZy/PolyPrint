using PolyPrint.AppData;
using PolyPrint.View.Pages;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PolyPrint.View.Windows
{
    public partial class MainWindow : Window
    {
        #region Инициализация

        public MainWindow()
        {
            InitializeComponent();

            ClientsButton.Click += ClientsButton_Click;
            OrdersButton.Click += OrdersButton_Click;
            EquipmentButton.Click += EquipmentButton_Click;
            ServiceButton.Click += ServiceButton_Click;
            PartsButton.Click += PartsButton_Click;
            JournalButton.Click += JournalButton_Click;

            NavigateToClients();
        }

        #endregion

        #region Навигация

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToClients();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToOrders();
        }

        private void EquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToEquipment();
        }

        private void ServiceButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToService();
        }

        private void PartsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToParts();
        }

        private void JournalButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToJournal();
        }

        #endregion

        #region Методы навигации

        private void NavigateToClients()
        {
            MainFrame.Navigate(new AddClientPage());
        }

        private void NavigateToOrders()
        {
            MainFrame.Navigate(new AddOrderPage());
        }

        private void NavigateToEquipment()
        {
            MainFrame.Navigate(new AddEquipmentPage());
        }

        private void NavigateToService()
        {
            MainFrame.Navigate(new AddServiceRequestPage());
        }

        private void NavigateToParts()
        {
            MainFrame.Navigate(new AddPartPage());
        }

        private void NavigateToJournal()
        {
            MainFrame.Navigate(new JournalPage());
        }

        #endregion
    }
}