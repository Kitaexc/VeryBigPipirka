using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Policy;
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
    /// Логика взаимодействия для Page5.xaml
    /// </summary>
    public partial class Page5 : Page
    {
        UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page5()
        {
            InitializeComponent();
            Cheque.ItemsSource = context.Cheque.Include("Products").Include("Orders").ToList();

            this.Loaded += (s, e) =>
            {
                var products = context.Products.ToList();
                foreach (var prod in products)
                {
                    Three.Items.Add(new ComboBoxItem() { Content = prod.ProductName, Tag = prod.ID_Product });
                }

                var orders = context.Orders.ToList();
                foreach (var ord in orders)
                {
                    Four.Items.Add(new ComboBoxItem() { Content = ord.DateOrder, Tag = ord.ID_Order });
                }
            };
            Cheque.SelectionChanged += Cheque_SelectionChanged;
        }

        private void Four_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsTextAllowed(e.Text))
            {
                e.Handled = true;
            }
        }

        private void Four_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextAllowed(string text)
        {
            return text.All(char.IsDigit);
        }

        private void Cheque_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cheque.SelectedItem is Cheque selectedAuth)
            {

                One.Text = selectedAuth.TotalPrice.ToString();
                Two.Text = selectedAuth.DateChequeIssue;

                var comboBoxItem = Three.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.Product_ID);
                if (comboBoxItem != null)
                {
                    Three.SelectedItem = comboBoxItem;
                    Three.Text = comboBoxItem.Content.ToString();
                }
                else
                {
                    Three.SelectedItem = null;
                    Three.Text = string.Empty;
                }

                var comboBoxItem2 = Four.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.Order_ID);
                if (comboBoxItem2 != null)
                {
                    Four.SelectedItem = comboBoxItem2;
                    Four.Text = comboBoxItem2.Content.ToString();
                }
                else
                {
                    Four.SelectedItem = null;
                    Four.Text = string.Empty;
                }
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = Three.SelectedItem as ComboBoxItem;
            var selectedOrder = Four.SelectedItem as ComboBoxItem;
            var totalPrice = One.Text;

            var dateIssue = DateTime.Now.ToString("yyyy-MM-dd");

            if (selectedProduct == null || selectedOrder == null || string.IsNullOrWhiteSpace(totalPrice))
            {
                MessageBox.Show("Необходимо заполнить все поля, кроме даты выдачи чека.");
                return;
            }

            decimal price;
            if (!decimal.TryParse(totalPrice, out price))
            {
                MessageBox.Show("Итоговая цена должна быть числом.");
                return;
            }

            try
            {
                var newCheque = new Cheque
                {
                    Product_ID = (int)selectedProduct.Tag,
                    Order_ID = (int)selectedOrder.Tag,
                    TotalPrice = price,
                    DateChequeIssue = dateIssue
                };

                context.Cheque.Add(newCheque);
                context.SaveChanges();

                Cheque.ItemsSource = context.Cheque.Include("Products").Include("Orders").ToList();

                One.Clear();
                Three.SelectedIndex = -1;
                Four.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении чека: {ex.Message}");
            }
        }


        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (Cheque.SelectedItem == null)
            {
                MessageBox.Show("Выберите чек для редактирования.");
                return;
            }

            var selectedCheque = Cheque.SelectedItem as Cheque;
            var totalPrice = One.Text;
            var dateChequeIssue = Two.SelectedDate.HasValue ? Two.SelectedDate.Value.ToString("yyyy-MM-dd") : null;
            var selectedProductItem = Three.SelectedItem as ComboBoxItem;
            var selectedOrderItem = Four.SelectedItem as ComboBoxItem;

            if (selectedProductItem == null || selectedOrderItem == null || string.IsNullOrWhiteSpace(totalPrice) || dateChequeIssue == null)
            {
                MessageBox.Show("Необходимо заполнить все поля.");
                return;
            }

            try
            {
                var chequeInDb = context.Cheque.Find(selectedCheque.ID_Cheque);
                if (chequeInDb != null)
                {
                    chequeInDb.TotalPrice = decimal.Parse(totalPrice);
                    chequeInDb.DateChequeIssue = dateChequeIssue;
                    chequeInDb.Product_ID = (int)selectedProductItem.Tag;
                    chequeInDb.Order_ID = (int)selectedOrderItem.Tag;

                    context.SaveChanges();

                    Cheque.ItemsSource = context.Cheque.Include("Products").Include("Orders").ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании чека: {ex.Message}");
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedCheque = Cheque.SelectedItem as Cheque;
            if (selectedCheque == null)
            {
                MessageBox.Show("Выберите чек для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот чек?", "Удаление чека", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var chequeInDb = context.Cheque.Find(selectedCheque.ID_Cheque);
                    if (chequeInDb != null)
                    {
                        context.Cheque.Remove(chequeInDb);
                        context.SaveChanges();
                        Cheque.ItemsSource = context.Cheque.Include("Products").Include("Orders").ToList();
                    }
                    else
                    {
                        MessageBox.Show("Чек не найден в базе данных.");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show("Нельзя удалить чек, так как он связан с другими данными в базе.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при попытке удаления чека: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
