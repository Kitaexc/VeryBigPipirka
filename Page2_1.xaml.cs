using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
    /// Логика взаимодействия для Page2_1.xaml
    /// </summary>
    public partial class Page2_1 : Page
    {
        private UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page2_1()
        {
            InitializeComponent();
            PriceHistory.ItemsSource = context.PriceHistory.ToList();
        }

        private void PriceHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PriceHistory.SelectedItem == null) return;

            var row = (PriceHistory)PriceHistory.SelectedItem;

            One.Text = row.OldPrice.ToString(); 
            Two.Text = row.NewPrice.ToString();

            if (PriceHistory.SelectedItem is PriceHistory selectedHistory)
            {
                Three.SelectedDate = DateTime.Parse(selectedHistory.DateRevisions);
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Two.Text))
            {
                MessageBox.Show("Необходимо заполнить поле 'Новая цена'.");
                return;
            }

            if (decimal.TryParse(Two.Text, out decimal newPrice))
            {
                var newPriceHistory = new PriceHistory
                {
                    OldPrice = newPrice,
                    NewPrice = newPrice,
                    DateRevisions = DateTime.Now.ToString("yyyy-MM-dd") 
                };

                context.PriceHistory.Add(newPriceHistory);
                context.SaveChanges();

                PriceHistory.ItemsSource = context.PriceHistory.ToList();
            }
            else
            {
                MessageBox.Show("Ошибка при преобразовании цен. Проверьте введенные данные.");
            }
        }


        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (PriceHistory.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись для редактирования.");
                return;
            }

            var selectedPriceHistory = PriceHistory.SelectedItem as PriceHistory;

            if (string.IsNullOrWhiteSpace(Two.Text))
            {
                MessageBox.Show("Необходимо заполнить поле 'Новая цена'.");
                return;
            }

            try
            {
                if (decimal.TryParse(Two.Text, out decimal newPrice))
                {
                    var priceHistoryInDb = context.PriceHistory.Find(selectedPriceHistory.ID_PriceHistory);
                    if (priceHistoryInDb != null)
                    {
                        priceHistoryInDb.OldPrice = priceHistoryInDb.NewPrice;
                        priceHistoryInDb.NewPrice = newPrice;
                        priceHistoryInDb.DateRevisions = DateTime.Now.ToString("yyyy-MM-dd");

                        context.SaveChanges();

                        PriceHistory.ItemsSource = context.PriceHistory.ToList();
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка в значении новой цены. Проверьте введенные данные.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании записи: {ex.Message}");
            }
        }



        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedPriceHistory = PriceHistory.SelectedItem as PriceHistory;
            if (selectedPriceHistory == null)
            {
                MessageBox.Show("Выберите запись для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var priceHistoryInDb = context.PriceHistory.Find(selectedPriceHistory.ID_PriceHistory);
                    if (priceHistoryInDb != null)
                    {
                        context.PriceHistory.Remove(priceHistoryInDb);
                        context.SaveChanges();
                        PriceHistory.ItemsSource = context.PriceHistory.ToList();
                    }
                    else
                    {
                        MessageBox.Show("Запись не найдена в базе данных.");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show("Нельзя удалить эту запись, так как она связана с другими данными в базе.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при попытке удаления записи: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
