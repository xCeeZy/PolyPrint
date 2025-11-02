using PolyPrint.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint.View.Pages
{
    public partial class AddPartPage : Page
    {
        public AddPartPage()
        {
            InitializeComponent();
            LoadParts();
        }

        private void LoadParts()
        {
            var items = App.db.Parts
                .ToList()
                .Select(p => new
                {
                    ID = p.ID_Part,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                })
                .OrderBy(p => p.Name)
                .ToList();

            PartsGrid.ItemsSource = items;
        }

        private void SavePartButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название запчасти", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string quantityText = QuantityTextBox.Text?.Trim() ?? string.Empty;
            if (!int.TryParse(quantityText, out int quantity))
            {
                MessageBox.Show("Количество должно быть целым числом", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string priceText = PriceTextBox.Text?.Trim() ?? string.Empty;
            if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal price) &&
                !decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out price))
            {
                MessageBox.Show("Стоимость указана некорректно", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Parts part = new Parts
            {
                Name = name,
                Quantity = quantity,
                Price = price
            };

            try
            {
                App.db.Parts.Add(part);
                App.db.SaveChanges();
                MessageBox.Show("Запчасть сохранена", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadParts();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить запчасть. {ex.Message}", "PolyPrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearPartForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            NameTextBox.Text = string.Empty;
            QuantityTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            NameTextBox.Focus();
        }
    }
}
