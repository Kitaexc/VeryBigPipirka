using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using VeryBigPipirka;
namespace VeryBigPipirka
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = 630;
            this.Top = 200;
        }

        private void Sing_Click_1(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Password;
            string hashedPassword = HashPassword(password);

            string role = GetUserRole(login, hashedPassword);

            switch (role)
            {
                case "Администратор":
                    Window1 adminWindow = new Window1();
                    adminWindow.Show();
                    this.Close();
                    break;
                case "Склад менеджер":
                    Window2 cashierWindow = new Window2();
                    cashierWindow.Show();
                    this.Close();
                    break;
                case "Кассир":
                    Window3 clientWindow = new Window3();
                    clientWindow.Show();
                    this.Close();
                    break;
                default:
                    MessageBox.Show("Неверный логин или пароль");
                    break;
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public string GetUserRole(string userLogin, string passwordHash)
        {
            string connectionString = @"Data Source=DESKTOP-LA63VO7\SQLEXPRESS;Initial Catalog=UNLV_CORPORATION;Integrated Security=True";

            string role = string.Empty;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Role_ID FROM Authorisation WHERE UserLogin = @login AND PasswordHash = @passwordHash";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", userLogin);
                command.Parameters.AddWithValue("@passwordHash", passwordHash);

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    int roleId = (int)result;
                    query = "SELECT RoleName FROM Roles WHERE ID_Role = @roleId";

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@roleId", roleId);

                    result = command.ExecuteScalar();

                    if (result != null)
                    {
                        role = result.ToString();
                    }
                }
            }
            return role;
        }
    }
}