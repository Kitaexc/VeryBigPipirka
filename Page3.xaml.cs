using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        private UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();

        public Page3()
        {
            InitializeComponent();
            Authorisation.ItemsSource = context.Authorisation.Include("Roles").ToList();
            this.Loaded += (s, e) =>
            {
                var roles = context.Roles.ToList();
                foreach (var role in roles)
                {
                    Three.Items.Add(new ComboBoxItem() { Content = role.RoleName, Tag = role.ID_Role });
                }
            };
            
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        private void Authorisation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Authorisation.SelectedItem is Authorisation selectedAuth)
            {
                One.Text = selectedAuth.UserLogin;
                Two.Text = selectedAuth.PasswordHash;

                var comboBoxItem = Three.Items
                                    .Cast<ComboBoxItem>()
                                    .FirstOrDefault(i => i.Tag != null && (int)i.Tag == selectedAuth.Role_ID);
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
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (Authorisation.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.");
                return;
            }

            if (Authorisation.SelectedItem is Authorisation selectedAuth)
            {
                var userInDb = context.Authorisation.FirstOrDefault(a => a.ID_Authorisation == selectedAuth.ID_Authorisation);
                if (userInDb != null)
                {
                    string newLogin = One.Text.Trim();
                    string newPassword = Two.Text;
                    if (!string.Equals(userInDb.UserLogin, newLogin, StringComparison.OrdinalIgnoreCase) && context.Authorisation.Any(a => a.UserLogin.Equals(newLogin, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.");
                        return;
                    }

                    userInDb.UserLogin = newLogin;

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        if (newPassword.Length != 64)
                        {
                            userInDb.PasswordHash = ComputeSha256Hash(newPassword);
                        }
                        else if (newPassword != userInDb.PasswordHash)
                        {
                            userInDb.PasswordHash = newPassword;
                        }
                    }

                    var selectedRole = Three.SelectedItem as ComboBoxItem;
                    if (selectedRole != null)
                    {
                        int roleId = (int)selectedRole.Tag;
                        userInDb.Role_ID = roleId;
                    }

                    try
                    {
                        context.SaveChanges();
                        Authorisation.ItemsSource = context.Authorisation.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка при сохранении данных: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Ошибка: Пользователь не выбран или объект не соответствует ожидаемому типу.");
            }
        }



        private void create_Click(object sender, RoutedEventArgs e)
        {
            string userLogin = One.Text.Trim();
            string password = Two.Text;

            if (string.IsNullOrEmpty(userLogin))
            {
                MessageBox.Show("Логин не может быть пустым.");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пароль не может быть пустым.");
                return;
            }

            var selectedRole = Three.SelectedItem as ComboBoxItem;
            if (selectedRole == null)
            {
                MessageBox.Show("Выберите роль.");
                return;
            }

            int roleId;
            if (!int.TryParse(selectedRole.Tag.ToString(), out roleId))
            {
                MessageBox.Show("Ошибка при получении идентификатора роли.");
                return;
            }

            var userExists = context.Authorisation.Any(a => a.UserLogin.Equals(userLogin, StringComparison.OrdinalIgnoreCase));
            if (userExists)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.");
                return;
            }

            var newAuth = new Authorisation
            {
                UserLogin = userLogin,
                PasswordHash = ComputeSha256Hash(password),
                Role_ID = roleId
            };

            context.Authorisation.Add(newAuth);

            try
            {

                context.SaveChanges();

                Authorisation.ItemsSource = context.Authorisation.ToList();

                One.Clear();
                Two.Clear();
                Three.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении пользователя: " + ex.Message);
            }
        }


        private void delite_Click(object sender, RoutedEventArgs e)
        {
            if (Authorisation.SelectedItem is Authorisation selectedAuth)
            {
                var authToDelete = context.Authorisation.FirstOrDefault(a => a.ID_Authorisation == selectedAuth.ID_Authorisation);
                if (authToDelete != null)
                {
                    try
                    {
                        context.Authorisation.Remove(authToDelete);

                        context.SaveChanges();

                        Authorisation.ItemsSource = context.Authorisation.ToList();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                    {
                        MessageBox.Show("Эту запись нельзя удалить, так как она связана с другими таблицами.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка при попытке удаления записи.");
                    }
                }
                else
                {
                    MessageBox.Show("Запись для удаления не найдена.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
            }
        }
    }
}
