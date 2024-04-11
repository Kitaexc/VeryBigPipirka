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
    /// Логика взаимодействия для Page3_1.xaml
    /// </summary>
    public partial class Page3_1 : Page
    {
        UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page3_1()
        {
            InitializeComponent();
            ProductImage.ItemsSource = context.ProductImage.ToList();
        }

        private void ProductImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductImage.SelectedItem == null) return;

            var selectedImage = (ProductImage)ProductImage.SelectedItem;

            One.Text = selectedImage.ImagePath;
                                               
            if (DateTime.TryParse(selectedImage.DateAdded, out DateTime dateAdded))
            {
                Two.SelectedDate = dateAdded;
            }
            else
            {
                MessageBox.Show("Дата добавлена в неверном формате");
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            if (!One.Text.StartsWith("https://disk.yandex.ru/i/"))
            {
                MessageBox.Show("Ссылка должна начинаться с 'https://disk.yandex.ru/i/'.");
                return;
            }

            if (string.IsNullOrWhiteSpace(One.Text))
            {
                MessageBox.Show("Путь к изображению должен быть заполнен.");
                return;
            }

            var newProductImage = new ProductImage
            {
                ImagePath = One.Text,
                DateAdded = DateTime.Now.ToString("yyyy-MM-dd")
            };

            context.ProductImage.Add(newProductImage);
            context.SaveChanges();

            ProductImage.ItemsSource = context.ProductImage.ToList();
        }


        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductImage.SelectedItem == null)
            {
                MessageBox.Show("Выберите изображение для редактирования.");
                return;
            }

            if (string.IsNullOrWhiteSpace(One.Text))
            {
                MessageBox.Show("Путь к изображению должен быть заполнен.");
                return;
            }

            if (!One.Text.StartsWith("https://disk.yandex.ru/i/"))
            {
                MessageBox.Show("Ссылка должна начинаться с 'https://disk.yandex.ru/i/'.");
                return;
            }

            var selectedProductImage = ProductImage.SelectedItem as ProductImage;

            try
            {
                var productImageInDb = context.ProductImage.Find(selectedProductImage.ID_ProductImage);
                if (productImageInDb != null)
                {
                    productImageInDb.ImagePath = One.Text;
                    context.SaveChanges();

                    ProductImage.ItemsSource = context.ProductImage.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании изображения: {ex.Message}");
            }
        }


        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedProductImage = ProductImage.SelectedItem as ProductImage;
            if (selectedProductImage == null)
            {
                MessageBox.Show("Выберите изображение для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить это изображение?", "Удаление изображения", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var productImageInDb = context.ProductImage.Find(selectedProductImage.ID_ProductImage);
                    if (productImageInDb != null)
                    {
                        context.ProductImage.Remove(productImageInDb);
                        context.SaveChanges();
                        ProductImage.ItemsSource = context.ProductImage.ToList();
                    }
                    else
                    {
                        MessageBox.Show("Изображение не найдено в базе данных.");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show("Нельзя удалить это изображение, так как оно связано с другими данными в базе.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при попытке удаления изображения: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
