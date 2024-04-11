using Microsoft.SqlServer.Management.Smo.Agent;
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
    /// Логика взаимодействия для Page1_1.xaml
    /// </summary>
    public partial class Page1_1 : Page
    {
        private UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page1_1()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(One, OnPaste);
            ProductCategory.ItemsSource = context.ProductCategory.ToList();
        }

        private void ProductCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductCategory.SelectedItem == null) return;

            var roles = (ProductCategory)ProductCategory.SelectedItem;

            One.Text = roles.CategoryName;
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = ProductCategory.SelectedItem as ProductCategory;
            if (selectedCategory != null)
            {
                if (context.Products.Any(p => p.Category_ID == selectedCategory.ID_Category))
                {
                    MessageBox.Show("Эту роль нельзя изменить, так как она связана с существующими записями в таблице Products.");
                    return;
                }

                var categoryInDb = context.ProductCategory.FirstOrDefault(pc => pc.ID_Category == selectedCategory.ID_Category);
                if (categoryInDb != null)
                {
                    categoryInDb.CategoryName = One.Text;

                    try
                    {
                        context.SaveChanges();
                        ProductCategory.ItemsSource = context.ProductCategory.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении роли: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите роль для редактирования.");
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = One.Text.Trim();

            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Имя категории не может быть пустым.");
                return;
            }

            var categoryExists = context.ProductCategory.Any(pc => pc.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (categoryExists)
            {
                MessageBox.Show("Категория с таким именем уже существует.");
                return;
            }

            var newCategory = new ProductCategory { CategoryName = categoryName };

            context.ProductCategory.Add(newCategory);

            try
            {
                context.SaveChanges();

                ProductCategory.ItemsSource = context.ProductCategory.ToList();

                One.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении категории: " + ex.Message);
            }
        }

        private void One_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsLetter);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsLetter))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = ProductCategory.SelectedItem as ProductCategory;
            if (selectedCategory == null)
            {
                MessageBox.Show("Выберите категорию для удаления.");
                return;
            }

            if (context.Products.Any(p => p.Category_ID == selectedCategory.ID_Category))
            {
                MessageBox.Show("Эту категорию нельзя удалить, так как она связана с существующими записями в таблице Products.");
                return;
            }

            try
            {
                context.ProductCategory.Remove(selectedCategory);
                context.SaveChanges();
                ProductCategory.ItemsSource = context.ProductCategory.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при попытке удаления категории: " + ex.Message);
            }
        }

    }
}
