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
    /// Логика взаимодействия для Page3_2.xaml
    /// </summary>
    public partial class Page3_2 : Page
    {
        UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page3_2()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(One, OnPaste);
            OrderStatus.ItemsSource = context.OrderStatus.ToList();
        }

        private void OrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderStatus.SelectedItem == null) return;

            var roles = (OrderStatus)OrderStatus.SelectedItem;

            One.Text = roles.StatusName;
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

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedStatus = OrderStatus.SelectedItem as OrderStatus;
            if (selectedStatus != null)
            {
                
                var statusInDb = context.OrderStatus.FirstOrDefault(os => os.ID_OrderStatus == selectedStatus.ID_OrderStatus);
                if (statusInDb != null)
                {
                    statusInDb.StatusName = One.Text;

                    try
                    {
                        context.SaveChanges();

                        OrderStatus.ItemsSource = context.OrderStatus.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении статуса заказа: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите статус заказа для редактирования.");
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            string statusName = One.Text.Trim();

            if (string.IsNullOrEmpty(statusName))
            {
                MessageBox.Show("Название статуса не может быть пустым.");
                return;
            }

            var statusExists = context.OrderStatus.Any(os => os.StatusName.Equals(statusName, StringComparison.OrdinalIgnoreCase));
            if (statusExists)
            {
                MessageBox.Show("Статус с таким названием уже существует.");
                return;
            }

            var newStatus = new OrderStatus { StatusName = statusName };
            context.OrderStatus.Add(newStatus);

            try
            {
                context.SaveChanges();

                OrderStatus.ItemsSource = context.OrderStatus.ToList();

                One.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении статуса заказа: " + ex.Message);
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedStatus = OrderStatus.SelectedItem as OrderStatus;
            if (selectedStatus == null)
            {
                MessageBox.Show("Выберите статус заказа для удаления.");
                return;
            }

            if (context.Orders.Any(o => o.OrderStatus_ID == selectedStatus.ID_OrderStatus))
            {
                MessageBox.Show("Этот статус заказа нельзя удалить, так как он связан с существующими заказами.");
                return;
            }

            try
            {
                context.OrderStatus.Remove(selectedStatus);
                context.SaveChanges();

                OrderStatus.ItemsSource = context.OrderStatus.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при попытке удаления статуса заказа: " + ex.Message);
            }
        }


    }
}
