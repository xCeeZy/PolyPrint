using PolyPrint.View.Pages;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PolyPrint.View.Windows
{
    public partial class MainWindow : Window
    {
        private readonly List<Button> _navigationButtons = new List<Button>();

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigated += MainFrame_Navigated;

            _navigationButtons.AddRange(new[]
            {
                JournalButton,
                ClientsButton,
                OrdersButton,
                EquipmentButton,
                ServiceButton,
                WorksButton,
                PartsButton
            });

            NavigateToPage(new JournalPage(), JournalButton, "Журнал");
        }

        private void NavigateToPage(Page page, Button sourceButton, string title)
        {
            if (page == null || sourceButton == null)
            {
                return;
            }

            MainFrame.Navigate(page);
            PageTitleTextBlock.Text = title;
            PageSubtitleTextBlock.Text = page.Tag as string ?? string.Empty;
            SetActiveNavigation(sourceButton);
        }

        private void SetActiveNavigation(Button activeButton)
        {
            foreach (Button button in _navigationButtons)
            {
                if (button == null)
                {
                    continue;
                }

                button.Tag = button == activeButton ? "Active" : null;
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                PageSubtitleTextBlock.Text = page.Tag as string ?? string.Empty;
            }
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new AddClientPage(), ClientsButton, "Клиенты");
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new AddOrderPage(), OrdersButton, "Заказы");
        }

        private void EquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new AddEquipmentPage(), EquipmentButton, "Оборудование");
        }

        private void ServiceButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new AddServiceRequestPage(), ServiceButton, "Сервисные заявки");
        }

        private void PartsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new AddPartPage(), PartsButton, "Запчасти");
        }

        private void JournalButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new JournalPage(), JournalButton, "Журнал");
        }

        private void WorksButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new AddWorkPage(), WorksButton, "Работы");
        }
    }
}
