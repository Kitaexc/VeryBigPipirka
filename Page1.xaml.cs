using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using System.IO;
namespace VeryBigPipirka
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private UNLV_CORPORATIONEntities4 context = new UNLV_CORPORATIONEntities4();
        public Page1()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(One, OnPaste);
            Role.ItemsSource = context.Roles.ToList();
        }

        private void Role_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Role.SelectedItem == null) return;

            var roles = (Roles)Role.SelectedItem;

            One.Text = roles.RoleName;
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
            var selectedRole = Role.SelectedItem as Roles;
            if (selectedRole != null)
            {
                if (context.Authorisation.Any(a => a.Role_ID == selectedRole.ID_Role))
                {
                    MessageBox.Show("Эту роль нельзя изменить, так как она связана с существующими записями в таблице Authorisation.");
                    return;
                }

                var roleInDb = context.Roles.FirstOrDefault(r => r.ID_Role == selectedRole.ID_Role);
                if (roleInDb != null)
                {
                    roleInDb.RoleName = One.Text;

                    try
                    {
                        context.SaveChanges();
                        Role.ItemsSource = context.Roles.ToList();
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
            string roleName = One.Text.Trim();

            if (string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show("Имя роли не может быть пустым.");
                return;
            }

            var roleExists = context.Roles.Any(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (roleExists)
            {
                MessageBox.Show("Роль с таким именем уже существует.");
                return;
            }

            var newRole = new Roles { RoleName = roleName };

            context.Roles.Add(newRole);

            try
            {
                context.SaveChanges();

                Role.ItemsSource = context.Roles.ToList();

                One.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при добавлении роли: " + ex.Message);
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedRole = Role.SelectedItem as Roles;
            if (selectedRole == null)
            {
                MessageBox.Show("Выберите роль для удаления.");
                return;
            }

            if (context.Authorisation.Any(a => a.Role_ID == selectedRole.ID_Role))
            {
                MessageBox.Show("Эту роль нельзя удалить, так как она связана с существующими записями в таблице Authorisation.");
                return;
            }

            try
            {
                context.Roles.Remove(selectedRole);
                context.SaveChanges();
                Role.ItemsSource = context.Roles.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при попытке удаления роли: " + ex.Message);
            }
        }

        public class CityJson
        {
            public string RoleName { get; set; }
        }


        private void Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jsonFilePath = @"C:\Users\UNLW_Deweloper\source\repos\VeryBigPipirka\VeryBigPipirka\Roles.json";

                var json = File.ReadAllText(jsonFilePath);
                var roles = JsonConvert.DeserializeObject<List<CityJson>>(json);

                if (roles == null)
                {
                    MessageBox.Show("Нет данных для импорта.");
                    return;
                }

                foreach (var roleJson in roles)
                {
                    var role = new Roles { RoleName = roleJson.RoleName };
                    context.Roles.Add(role);
                }

                context.SaveChanges();
                Role.ItemsSource = context.Roles.ToList();
                MessageBox.Show("Импорт завершен успешно.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте: {ex.Message}");
            }
        }

    }
}
