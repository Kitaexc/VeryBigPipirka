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
    /// Логика взаимодействия для Page4.xaml
    /// </summary>
    public partial class Page4 : Page
    {
        UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page4()
        {
            InitializeComponent();
            Two.DisplayDateStart = DateTime.Now.AddDays(1);
            PaymentMethod.ItemsSource = context.PaymentMethod.ToList();
            PaymentMethod.SelectionChanged += PaymentMethod_SelectionChanged;
        }

        private void One_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void One_PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsDigit) || text.Length > 16) 
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        private void PaymentMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentMethod.SelectedItem is PaymentMethod selectedPayment)
            {
                One.Text = selectedPayment.CardNumber;
                Two.SelectedDate = DateTime.Parse(selectedPayment.ExpiryDate);
                Three.Text = selectedPayment.CardholderName;
            }
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(One.Text) || Two.SelectedDate == null || string.IsNullOrEmpty(Three.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены.");
                return;
            }
            try
            {
                var newPaymentMethod = new PaymentMethod
                {
                    CardNumber = One.Text,
                    ExpiryDate = Two.SelectedDate.Value.ToString("yyyy-MM-dd"),
                    CardholderName = Three.Text
                };

                context.PaymentMethod.Add(newPaymentMethod);

                context.SaveChanges();

                PaymentMethod.ItemsSource = context.PaymentMethod.ToList();

                One.Clear();
                Two.SelectedDate = null;
                Three.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении метода оплаты: " + ex.Message);
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentMethod.SelectedItem == null)
            {
                MessageBox.Show("Выберите метод оплаты для редактирования.");
                return;
            }
            if (string.IsNullOrEmpty(One.Text) || Two.SelectedDate == null || string.IsNullOrEmpty(Three.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены.");
                return;
            }

            var selectedPaymentMethod = PaymentMethod.SelectedItem as PaymentMethod;
            if (selectedPaymentMethod != null)
            {
                try
                {

                    var paymentMethodInDb = context.PaymentMethod.Find(selectedPaymentMethod.ID_PaymentMethod);
                    if (paymentMethodInDb != null)
                    {
                        paymentMethodInDb.CardNumber = One.Text;
                        paymentMethodInDb.ExpiryDate = Two.SelectedDate.Value.ToString("yyyy-MM-dd");
                        paymentMethodInDb.CardholderName = Three.Text;


                        context.SaveChanges();

                        PaymentMethod.ItemsSource = context.PaymentMethod.ToList();

                        One.Clear();
                        Two.SelectedDate = null;
                        Three.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении метода оплаты:" + ex.Message);
                }
            }
        }

        private void Three_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsLetter(c) && (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z'));
        }

        private void Three_PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(c => char.IsLetter(c) && (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')))
                {
                    e.CancelCommand();
                }
                else
                {
                    ((TextBox)sender).Text += text.ToUpper();
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Three_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            int caretIndex = textBox.CaretIndex;
            textBox.Text = textBox.Text.ToUpper();
            textBox.CaretIndex = caretIndex; 
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedPaymentMethod = PaymentMethod.SelectedItem as PaymentMethod;
            if (selectedPaymentMethod == null)
            {
                MessageBox.Show("Выберите метод оплаты для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить выбранный метод оплаты?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {

                    context.PaymentMethod.Remove(selectedPaymentMethod);

                    context.SaveChanges();
                    PaymentMethod.ItemsSource = context.PaymentMethod.ToList();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show("Эту запись нельзя удалить, так как она используется в других частях программы.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при попытке удаления записи: " + ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}