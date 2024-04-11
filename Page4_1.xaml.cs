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

namespace VeryBigPipirka
{
    /// <summary>
    /// Логика взаимодействия для Page4_1.xaml
    /// </summary>
    public partial class Page4_1 : Page
    {
        UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page4_1()
        {
            InitializeComponent();
            Products.ItemsSource = context.Products.Include("ProductCategory").Include("PriceHistory").Include("ProductImage").ToList();

            this.Loaded += (s, e) =>
            {
                var productCategory = context.ProductCategory.ToList();
                foreach (var product in productCategory)
                {
                    One.Items.Add(new ComboBoxItem() { Content = product.CategoryName, Tag = product.ID_Category });
                }

                var priceHistory = context.PriceHistory.ToList();
                foreach (var price in priceHistory)
                {
                    Seven.Items.Add(new ComboBoxItem() { Content = price.NewPrice, Tag = price.ID_PriceHistory });
                }

                var productImage = context.ProductImage.ToList();
                foreach (var image in productImage)
                {
                    Six.Items.Add(new ComboBoxItem() { Content = image.ImagePath, Tag = image.ID_ProductImage });
                }
            };
            Products.SelectionChanged += Products_SelectionChanged;

        }

        private void Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Products.SelectedItem is Products selectedAuth)
            {
                var comboBoxItem1 = One.Items
                                        .Cast<ComboBoxItem>()
                                        .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.Category_ID);
                if (comboBoxItem1 != null)
                {
                    One.SelectedItem = comboBoxItem1;
                    One.Text = comboBoxItem1.Content.ToString();
                }
                else
                {
                    One.SelectedItem = null;
                    One.Text = string.Empty;
                }

                Two.Text = selectedAuth.ProductName;
                Three.Text = selectedAuth.DescriptionProducts;

                if (Products.SelectedItem == null) return;

                var row = (Products)Products.SelectedItem;

                Five.Text = row.QuantityInStock.ToString();

                var comboBoxItem = Six.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.ProductImage_ID);
                if (comboBoxItem != null)
                {
                    Six.SelectedItem = comboBoxItem;
                    Six.Text = comboBoxItem.Content.ToString();
                }
                else
                {
                    Six.SelectedItem = null;
                    Six.Text = string.Empty;
                }

                var comboBoxItem2 = Seven.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.PriceHistory_ID);
                if (comboBoxItem2 != null)
                {
                    Seven.SelectedItem = comboBoxItem2;
                    Seven.Text = comboBoxItem2.Content.ToString();
                }
                else
                {
                    Seven.SelectedItem = null;
                    Seven.Text = string.Empty;
                }
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = One.SelectedItem as ComboBoxItem;
            var productName = Two.Text;
            var productDescription = Three.Text;
            var selectedImage = Six.SelectedItem as ComboBoxItem;
            var selectedPriceHistory = Seven.SelectedItem as ComboBoxItem;

            if (selectedCategory == null || string.IsNullOrWhiteSpace(productName) ||
                string.IsNullOrWhiteSpace(productDescription) || selectedImage == null ||
                selectedPriceHistory == null || string.IsNullOrWhiteSpace(Five.Text))
            {
                MessageBox.Show("Необходимо заполнить все поля.");
                return;
            }

            if (int.TryParse(Five.Text, out int quantityInStock))
            {
                try
                {

                    var newProduct = new Products
                    {
                        Category_ID = (int)selectedCategory.Tag,
                        ProductName = productName,
                        DescriptionProducts = productDescription,
                        QuantityInStock = quantityInStock,
                        ProductImage_ID = selectedImage != null ? (int?)selectedImage.Tag : null,
                        PriceHistory_ID = selectedPriceHistory != null ? Convert.ToInt32(selectedPriceHistory.Tag) : 0,
                    };

                    context.Products.Add(newProduct);
                    context.SaveChanges();

                    Products.ItemsSource = context.Products.Include("ProductCategory").Include("PriceHistory").Include("ProductImage").ToList();

                    One.SelectedIndex = -1;
                    Two.Clear();
                    Three.Clear();
                    Five.Clear();
                    Six.SelectedIndex = -1;
                    Seven.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении продукта: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Ошибка при преобразовании цены и количества. Проверьте введенные данные.");
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = Products.SelectedItem as Products;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите продукт для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить выбранный продукт?", "Удаление продукта", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var productInDb = context.Products.Find(selectedProduct.ID_Product);
                    if (productInDb != null)
                    {
                        context.Products.Remove(productInDb);
                        context.SaveChanges();
                        Products.ItemsSource = context.Products.Include("ProductCategory").Include("PriceHistory").Include("ProductImage").ToList();
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show("Этот продукт нельзя удалить, так как он связан с другими данными в базе данных.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при удалении продукта: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = Products.SelectedItem as Products;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите продукт для редактирования.");
                return;
            }

            var selectedCategoryItem = One.SelectedItem as ComboBoxItem;
            var selectedImageItem = Six.SelectedItem as ComboBoxItem;
            var selectedPriceHistoryItem = Seven.SelectedItem as ComboBoxItem;
            var productName = Two.Text;
            var productDescription = Three.Text;
            var productQuantityText = Five.Text;

            if (selectedCategoryItem == null || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(productQuantityText))
            {
                MessageBox.Show("Все поля должны быть заполнены.");
                return;
            }

            if (!int.TryParse(productQuantityText, out int productQuantity))
            {
                MessageBox.Show("Цена и количество должны быть корректными числовыми значениями.");
                return;
            }

            try
            {
                var productInDb = context.Products.Find(selectedProduct.ID_Product);
                if (productInDb != null)
                {
                    productInDb.Category_ID = (int)selectedCategoryItem.Tag;
                    productInDb.ProductName = productName;
                    productInDb.DescriptionProducts = productDescription;
                    productInDb.QuantityInStock = productQuantity;
                    productInDb.ProductImage_ID = selectedImageItem != null ? (int?)selectedImageItem.Tag : null;
                    productInDb.PriceHistory_ID = selectedPriceHistoryItem != null ? Convert.ToInt32(selectedPriceHistoryItem.Tag) : 0;


                    context.SaveChanges();

                    Products.ItemsSource = context.Products.Include("ProductCategory").Include("PriceHistory").Include("ProductImage").ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании продукта: {ex.Message}");
            }
        }

    }
}
