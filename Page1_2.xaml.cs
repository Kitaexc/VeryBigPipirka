using Newtonsoft.Json;
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
using System.IO;


namespace VeryBigPipirka
{
    /// <summary>
    /// Логика взаимодействия для Page1_2.xaml
    /// </summary>
    public partial class Page1_2 : Page
    {
        UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page1_2()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(One, OnPaste);
            Cities.ItemsSource = context.Cities.ToList();
        }
        private void Cities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cities.SelectedItem == null) return;

            var roles = (Cities)Cities.SelectedItem;

            One.Text = roles.CityName;
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
            
            var selectedCity = Cities.SelectedItem as Cities;
            if (selectedCity != null)
            {
                if (context.ShippingAddress.Any(sa => sa.City_ID == selectedCity.ID_City))
                {
                    MessageBox.Show("Этот город нельзя изменить, так как он связан с существующими записями в таблице ShippingAddress.");
                    return;
                }

                var cityInDb = context.Cities.FirstOrDefault(c => c.ID_City == selectedCity.ID_City);
                if (cityInDb != null)
                {
                    cityInDb.CityName = One.Text;

                    try
                    {
                        context.SaveChanges(); 
                                               
                        Cities.ItemsSource = context.Cities.ToList(); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении города: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите город для редактирования.");
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            string cityName = One.Text.Trim();

            if (string.IsNullOrEmpty(cityName))
            {
                MessageBox.Show("Название города не может быть пустым.");
                return;
            }

            var cityExists = context.Cities.Any(c => c.CityName.Equals(cityName, StringComparison.OrdinalIgnoreCase));
            if (cityExists)
            {
                MessageBox.Show("Город с таким названием уже существует.");
                return;
            }

            var newCity = new Cities { CityName = cityName };
            context.Cities.Add(newCity);

            try
            {
                context.SaveChanges();

                Cities.ItemsSource = context.Cities.ToList();

                One.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении города: " + ex.Message);
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedCity = Cities.SelectedItem as Cities;
            if (selectedCity == null)
            {
                MessageBox.Show("Выберите город для удаления.");
                return;
            }

            if (context.ShippingAddress.Any(sa => sa.City_ID == selectedCity.ID_City))
            {
                MessageBox.Show("Этот город нельзя удалить, так как он связан с существующими записями в таблице ShippingAddress.");
                return;
            }

            try
            {
                context.Cities.Remove(selectedCity);
                context.SaveChanges(); 

                Cities.ItemsSource = context.Cities.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при попытке удаления города: " + ex.Message);
            }
        }

        public class CityJson
        {
            public string CityName { get; set; }
        }


        private void Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jsonFilePath = @"C:\Users\UNLW_Deweloper\source\repos\VeryBigPipirka\VeryBigPipirka\Cities.json"; 

                var json = File.ReadAllText(jsonFilePath);
                var cities = JsonConvert.DeserializeObject<List<CityJson>>(json);

                if (cities == null)
                {
                    MessageBox.Show("Нет данных для импорта.");
                    return;
                }

                foreach (var cityJson in cities)
                {
                    var city = new Cities { CityName = cityJson.CityName }; 
                    context.Cities.Add(city);
                }

                context.SaveChanges();
                Cities.ItemsSource = context.Cities.ToList();
                MessageBox.Show("Импорт завершен успешно.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте: {ex.Message}");
            }
        }

    }
}
