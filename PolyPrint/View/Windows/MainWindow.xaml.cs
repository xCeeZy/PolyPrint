using PolyPrint.View.Pages;
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

namespace PolyPrint.View.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new JournalPage());
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddClientPage());
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddOrderPage());
        }

        private void EquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddEquipmentPage());
        }

        private void ServiceButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddServiceRequestPage());
        }

        private void PartsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddPartPage());
        }

        private void JournalButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new JournalPage());
        }
    }
}
