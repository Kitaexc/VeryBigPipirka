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
    /// Логика взаимодействия для Page4_2.xaml
    /// </summary>
    public partial class Page4_2 : Page
    {
        UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page4_2()
        {
            InitializeComponent();
            Orders.ItemsSource = context.Orders.Include("Users").Include("ShippingAddress").Include("OrderStatus").Include("Products").ToList();

            this.Loaded += (s, e) =>
            {
                var users = context.Users.ToList();
                foreach (var auth in users)
                {
                    One.Items.Add(new ComboBoxItem() { Content = auth.Email, Tag = auth.ID_User });
                }

                var shippingAddress = context.ShippingAddress.ToList();
                foreach (var shipp in shippingAddress)
                {
                    Three.Items.Add(new ComboBoxItem() { Content = shipp.IndexAddress, Tag = shipp.ID_Address });
                }

                var orderStatus = context.OrderStatus.ToList();
                foreach (var order in orderStatus)
                {
                    Four.Items.Add(new ComboBoxItem() { Content = order.StatusName, Tag = order.ID_OrderStatus });
                }

                var products = context.Products.ToList();
                foreach (var prod in products)
                {
                    Five.Items.Add(new ComboBoxItem() { Content = prod.ProductName, Tag = prod.ID_Product });
                }
            };
            Orders.SelectionChanged += Orders_SelectionChanged;
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
        private void Orders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Orders.SelectedItem is Orders selectedAuth)
            {
                var comboBoxItem1 = One.Items
                                        .Cast<ComboBoxItem>()
                                        .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.User_ID);
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

                Two.Text = selectedAuth.DateOrder;

                var comboBoxItem = Three.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.Address_ID);
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
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.OrderStatus_ID);
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

                var comboBoxItem3 = Five.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.Product_ID);
                if (comboBoxItem3 != null)
                {
                    Five.SelectedItem = comboBoxItem3;
                    Five.Text = comboBoxItem3.Content.ToString();
                }
                else
                {
                    Five.SelectedItem = null;
                    Five.Text = string.Empty;
                }

                Six.Text = selectedAuth.ProductsOnOrder.ToString();
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = One.SelectedItem as ComboBoxItem;
            var selectedAddress = Three.SelectedItem as ComboBoxItem;
            var selectedOrderStatus = Four.SelectedItem as ComboBoxItem;
            var selectedProduct = Five.SelectedItem as ComboBoxItem;

            var dateOrder = DateTime.Now.ToString("yyyy-MM-dd");

            if (selectedUser == null || selectedAddress == null || selectedOrderStatus == null || selectedProduct == null)
            {
                MessageBox.Show("Необходимо заполнить все поля, кроме даты.");
                return;
            }

            try
            {
                var newOrder = new Orders
                {
                    User_ID = (int)selectedUser.Tag,
                    Address_ID = (int)selectedAddress.Tag,
                    OrderStatus_ID = (int)selectedOrderStatus.Tag,
                    Product_ID = (int)selectedProduct.Tag,
                    DateOrder = dateOrder,
                    ProductsOnOrder = int.Parse(Six.Text)
                };

                context.Orders.Add(newOrder);
                context.SaveChanges();

                Orders.ItemsSource = context.Orders.Include("Users").Include("ShippingAddress").Include("OrderStatus").Include("Products").ToList();

                One.SelectedIndex = -1;
                Three.SelectedIndex = -1;
                Four.SelectedIndex = -1;
                Five.SelectedIndex = -1;
                Six.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заказа: {ex.Message}");
            }
        }


        private void edit_Click(object sender, RoutedEventArgs e)
        {

            if (Orders.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ для редактирования.");
                return;
            }

            var selectedOrder = Orders.SelectedItem as Orders;


            var selectedUserItem = One.SelectedItem as ComboBoxItem;
            var selectedAddressItem = Three.SelectedItem as ComboBoxItem;
            var selectedStatusItem = Four.SelectedItem as ComboBoxItem;
            var selectedProductItem = Five.SelectedItem as ComboBoxItem;
            var dateOrder = Two.Text;
            var productsOnOrder = Six.Text;

            if (selectedUserItem == null || selectedAddressItem == null || selectedStatusItem == null || string.IsNullOrWhiteSpace(dateOrder) || string.IsNullOrWhiteSpace(productsOnOrder))
            {
                MessageBox.Show("Необходимо заполнить все поля.");
                return;
            }

            try
            {
                var orderInDb = context.Orders.Find(selectedOrder.ID_Order);
                if (orderInDb != null)
                {
                    orderInDb.User_ID = (int)selectedUserItem.Tag;
                    orderInDb.Address_ID = (int)selectedAddressItem.Tag;
                    orderInDb.OrderStatus_ID = (int)selectedStatusItem.Tag;
                    orderInDb.Product_ID = (int)selectedProductItem.Tag;
                    orderInDb.DateOrder = dateOrder;
                    orderInDb.ProductsOnOrder = int.Parse(productsOnOrder);

                    context.SaveChanges();

                    Orders.ItemsSource = context.Orders.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании заказа: {ex.Message}");
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = Orders.SelectedItem as Orders;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот заказ?", "Удаление заказа", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var orderInDb = context.Orders.Find(selectedOrder.ID_Order);
                    if (orderInDb != null)
                    {
                        context.Orders.Remove(orderInDb);
                        context.SaveChanges();
                        Orders.ItemsSource = context.Orders.Include("Users").Include("ShippingAddress").Include("OrderStatus").Include("Products").ToList();
                    }
                    else
                    {
                        MessageBox.Show("Заказ не найден в базе данных.");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show("Нельзя удалить заказ, так как он связан с другими данными в базе.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при попытке удаления заказа: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
