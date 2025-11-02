using PolyPrint.AppData;
using PolyPrint.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PolyPrint.View.Pages
{
    public partial class AddPartPage : Page
    {
        private static readonly char MetadataSeparator = '|';

        public AddPartPage()
        {
            InitializeComponent();
            LoadParts();
        }

        private void LoadParts()
        {
            var items = App.db.Parts
                .ToList()
                .Select(p =>
                {
                    ParseMetadata(p.Name, out string partName, out string article, out string supplier);
                    return new
                    {
                        ID = p.ID_Part,
                        Name = partName,
                        Article = article,
                        Supplier = supplier,
                        Quantity = p.Quantity,
                        Price = p.Price
                    };
                })
                .OrderBy(p => p.Name)
                .ToList();

            PartsGrid.ItemsSource = items;
        }

        private void SavePartButton_Click(object sender, RoutedEventArgs e)
        {
            string name = StringHelper.CapitalizeWords(NameTextBox.Text);
            string article = StringHelper.SanitizeForMetadata(ArticleTextBox.Text, MetadataSeparator);
            string supplier = StringHelper.SanitizeForMetadata(SupplierTextBox.Text, MetadataSeparator);

            if (!ValidationHelper.RequireNotEmpty(new Dictionary<string, string>
                {
                    { "Наименование", name },
                    { "Артикул", article },
                    { "Поставщик", supplier }
                }, out string requiredError))
            {
                DialogHelper.ShowWarning(requiredError);
                return;
            }

            if (!ValidationHelper.TryParseInt(StringHelper.Normalize(QuantityTextBox.Text), "Количество", out int quantity, out string quantityError))
            {
                DialogHelper.ShowWarning(quantityError);
                return;
            }

            if (!ValidationHelper.TryParseDecimal(StringHelper.Normalize(PriceTextBox.Text), "Цена", out decimal price, out string priceError))
            {
                DialogHelper.ShowWarning(priceError);
                return;
            }

            Parts part = new Parts
            {
                Name = ComposeMetadata(name, article, supplier),
                Quantity = quantity,
                Price = price
            };

            if (DbHelper.SaveEntity(part, (db, entity) => db.Parts.Add(entity), out string errorMessage))
            {
                DialogHelper.ShowSuccess("Запчасть сохранена.");
                LoadParts();
                ClearForm();
            }
            else
            {
                DialogHelper.ShowError($"Не удалось сохранить запчасть. {errorMessage}");
            }
        }

        private void ClearPartForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            NameTextBox.Text = string.Empty;
            ArticleTextBox.Text = string.Empty;
            SupplierTextBox.Text = string.Empty;
            QuantityTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            NameTextBox.Focus();
        }

        private static string ComposeMetadata(string name, string article, string supplier)
        {
            string sanitizedName = StringHelper.SanitizeForMetadata(name, MetadataSeparator);
            string sanitizedArticle = StringHelper.SanitizeForMetadata(article, MetadataSeparator);
            string sanitizedSupplier = StringHelper.SanitizeForMetadata(supplier, MetadataSeparator);
            return string.Join(MetadataSeparator.ToString(), new[] { sanitizedName, sanitizedArticle, sanitizedSupplier });
        }

        private static void ParseMetadata(string storedValue, out string name, out string article, out string supplier)
        {
            name = storedValue;
            article = string.Empty;
            supplier = string.Empty;

            if (string.IsNullOrWhiteSpace(storedValue))
            {
                name = string.Empty;
                return;
            }

            string[] parts = storedValue.Split(MetadataSeparator);
            if (parts.Length == 1)
            {
                name = parts[0];
                return;
            }

            if (parts.Length > 0)
            {
                name = parts[0];
            }

            if (parts.Length > 1)
            {
                article = parts[1];
            }

            if (parts.Length > 2)
            {
                supplier = parts[2];
            }
        }
    }
}
