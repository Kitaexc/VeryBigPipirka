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
    /// Логика взаимодействия для Page2_2.xaml
    /// </summary>
    public partial class Page2_2 : Page
    {
        UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page2_2()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(One, OnPaste);
            DataObject.AddPastingHandler(Two, OnPaste);
            ShippingAddress.ItemsSource = context.ShippingAddress.Include("Cities").ToList();
            this.Loaded += (s, e) =>
            {
                var ciryMethods = context.Cities.ToList();
                foreach (var city in ciryMethods)
                {
                    Three.Items.Add(new ComboBoxItem() { Content = city.CityName, Tag = city.ID_City });
                }
            };
            ShippingAddress.SelectionChanged += ShippingAddress_SelectionChanged;
        }

        private void One_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsLetter);
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

        private void ShippingAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShippingAddress.SelectedItem is ShippingAddress selectedAuth)
            {
                One.Text = selectedAuth.FullName;
                Two.Text = selectedAuth.Street;

                var comboBoxItem = Three.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.City_ID);
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
                Four.Text = selectedAuth.IndexAddress.ToString();
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            var selectedCity = Three.SelectedItem as ComboBoxItem;
            var fullName = One.Text;
            var street = Two.Text;
            var indexAddressStr = Four.Text;

            if (selectedCity == null || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(indexAddressStr))
            {
                MessageBox.Show("Необходимо заполнить все поля.");
                return;
            }

            int indexAddress;
            if (!int.TryParse(indexAddressStr, out indexAddress))
            {
                MessageBox.Show("Индекс должен быть числом.");
                return;
            }

            try
            {
                var newShippingAddress = new ShippingAddress
                {
                    City_ID = (int)selectedCity.Tag,
                    FullName = fullName,
                    Street = street,
                    IndexAddress = indexAddress
                };

                context.ShippingAddress.Add(newShippingAddress);
                context.SaveChanges();
                ShippingAddress.ItemsSource = context.ShippingAddress.ToList();

                One.Clear();
                Two.Clear();
                Three.SelectedIndex = -1;
                Four.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении адреса доставки: {ex.Message}");
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (ShippingAddress.SelectedItem == null)
            {
                MessageBox.Show("Выберите адрес для редактирования.");
                return;
            }

            var selectedAddress = ShippingAddress.SelectedItem as ShippingAddress;
            var fullName = One.Text;
            var street = Two.Text;
            var selectedCityItem = Three.SelectedItem as ComboBoxItem;
            if (int.TryParse(Four.Text, out int indexAddress) == false)
            {
                MessageBox.Show("Индекс должен быть числом.");
                return;
            }

            if (selectedCityItem == null || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(street) || indexAddress <= 0)
            {
                MessageBox.Show("Необходимо заполнить все поля.");
                return;
            }

            try
            {
                var addressInDb = context.ShippingAddress.Find(selectedAddress.ID_Address);
                if (addressInDb != null)
                {
                    addressInDb.FullName = fullName;
                    addressInDb.Street = street;
                    addressInDb.City_ID = (int)selectedCityItem.Tag;
                    addressInDb.IndexAddress = indexAddress;

                    context.SaveChanges();

                    ShippingAddress.ItemsSource = context.ShippingAddress.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании адреса: {ex.Message}");
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedAddress = ShippingAddress.SelectedItem as ShippingAddress;
            if (selectedAddress == null)
            {
                MessageBox.Show("Выберите адрес для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот адрес?", "Удаление адреса", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var addressInDb = context.ShippingAddress.Find(selectedAddress.ID_Address);
                    if (addressInDb != null)
                    {
                        context.ShippingAddress.Remove(addressInDb);
                        context.SaveChanges();
                        ShippingAddress.ItemsSource = context.ShippingAddress.Include("Cities").ToList(); 
                    }
                    else
                    {
                        MessageBox.Show("Адрес не найден в базе данных.");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show("Нельзя удалить адрес, так как он связан с другими данными в базе.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при попытке удаления адреса: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
