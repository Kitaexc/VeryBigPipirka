using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        private UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page2()
        {
            InitializeComponent();
            Users.ItemsSource = context.Users.Include("Authorisation").Include("PaymentMethod").Include("Roles").ToList();
            this.Loaded += (s, e) =>
            {
                var authorisations = context.Authorisation.ToList();
                foreach (var auth in authorisations)
                {
                    One.Items.Add(new ComboBoxItem() { Content = auth.UserLogin, Tag = auth.ID_Authorisation });
                }

                var paymentMethods = context.PaymentMethod.ToList();
                foreach (var payment in paymentMethods)
                {
                    Three.Items.Add(new ComboBoxItem() { Content = payment.CardholderName, Tag = payment.ID_PaymentMethod });
                }

                var roles = context.Roles.ToList();
                foreach (var role in roles)
                {
                    Four.Items.Add(new ComboBoxItem() { Content = role.RoleName, Tag = role.ID_Role });
                }
            };
            Users.SelectionChanged += Users_SelectionChanged;
        }



        private void Users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Users.SelectedItem is Users selectedAuth)
            {
                var comboBoxItem1 = One.Items
                                        .Cast<ComboBoxItem>()
                                        .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.Authorisation_ID);
                if( comboBoxItem1 != null )
                {
                    One.SelectedItem = comboBoxItem1;
                    One.Text = comboBoxItem1.Content.ToString();
                }
                else
                {
                    One.SelectedItem = null;
                    One.Text = string.Empty;
                }

                Two.Text = selectedAuth.Email;

                var comboBoxItem = Three.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.PaymentMethod_ID);
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
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.Role_ID);
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
            var selectedAuthorisation = One.SelectedItem as ComboBoxItem;
            var selectedPaymentMethod = Three.SelectedItem as ComboBoxItem;
            var selectedRole = Four.SelectedItem as ComboBoxItem;
            var email = Two.Text;

            if (selectedAuthorisation == null || selectedPaymentMethod == null || selectedRole == null || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Необходимо заполнить все поля.");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Введён некорректный адрес электронной почты.");
                return;
            }

            try
            {
                var newUser = new Users
                {
                    Authorisation_ID = (int)selectedAuthorisation.Tag,
                    PaymentMethod_ID = (int)selectedPaymentMethod.Tag,
                    Role_ID = (int)selectedRole.Tag,
                    Email = email
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                Users.ItemsSource = context.Users.Include("Authorisation").Include("PaymentMethod").Include("Roles").ToList();

                One.SelectedIndex = -1;
                Two.Clear();
                Three.SelectedIndex = -1;
                Four.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}");
            }
        }


        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (Users.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования.");
                return;
            }

            var selectedUser = Users.SelectedItem as Users;
            var selectedAuthorisationItem = One.SelectedItem as ComboBoxItem;
            var selectedPaymentMethodItem = Three.SelectedItem as ComboBoxItem;
            var selectedRoleItem = Four.SelectedItem as ComboBoxItem;
            var email = Two.Text;

            if (selectedAuthorisationItem == null || selectedPaymentMethodItem == null || selectedRoleItem == null || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Необходимо заполнить все поля.");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Введён некорректный адрес электронной почты.");
                return;
            }

            try
            {
                var userInDb = context.Users.Find(selectedUser.ID_User);
                if (userInDb != null)
                {
                    userInDb.Authorisation_ID = (int)selectedAuthorisationItem.Tag;
                    userInDb.PaymentMethod_ID = (int)selectedPaymentMethodItem.Tag;
                    userInDb.Role_ID = (int)selectedRoleItem.Tag;
                    userInDb.Email = email;

                    context.SaveChanges();

                    Users.ItemsSource = context.Users.Include("Authorisation").Include("PaymentMethod").Include("Roles").ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании пользователя: {ex.Message}");
            }
        }


        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = Users.SelectedItem as Users;
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?", "Удаление пользователя", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var userInDb = context.Users.Find(selectedUser.ID_User);
                    if (userInDb != null)
                    {
                        context.Users.Remove(userInDb);
                        context.SaveChanges();
                        Users.ItemsSource = context.Users.ToList();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден в базе данных.");
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show("Нельзя удалить пользователя, так как он связан с другими данными в базе.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при попытке удаления пользователя: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            var idn = new System.Globalization.IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Неверный домен электронной почты.");
            }
            return match.Groups[1].Value + domainName;
        }

    }
}
