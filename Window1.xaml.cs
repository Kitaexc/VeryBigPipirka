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
using System.Windows.Shapes;

namespace VeryBigPipirka
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void bek_Click(object sender, RoutedEventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string databaseName = "UNLV_CORPORATION";
            string fileName = $"Backup_{databaseName}_{DateTime.Now:yyyyMMddHHmmss}.bak";
            string backupPath = System.IO.Path.Combine(desktopPath, fileName);

            string connectionString = @"Data Source=DESKTOP-LA63VO7\SQLEXPRESS;Initial Catalog=UNLV_CORPORATION;Integrated Security=True";

            string sqlCommand = $"BACKUP DATABASE [{databaseName}] TO DISK = '{backupPath}' WITH FORMAT;";

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sqlCommand, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show($"Бекап базы данных успешно сохранен: {backupPath}", "Резервное копирование", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании бекапа базы данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Spisok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Spisok.SelectedIndex)
            {
                case 0:
                    InTable.Navigate(new Page());
                    Exit.Visibility = Visibility.Visible;
                    Info.Visibility = Visibility.Visible;
                    bek.Visibility = Visibility.Visible;
                    Kassa.Visibility = Visibility.Visible;
                    Manager.Visibility = Visibility.Visible;
                    break;
                case 1:
                    InTable.Navigate(new Page1());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    bek.Visibility = Visibility.Hidden;
                    Kassa.Visibility = Visibility.Hidden;
                    Manager.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    InTable.Navigate(new Page2());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    bek.Visibility = Visibility.Hidden;
                    Kassa.Visibility = Visibility.Hidden;
                    Manager.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    InTable.Navigate(new Page3());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    bek.Visibility = Visibility.Hidden;
                    Kassa.Visibility = Visibility.Hidden;
                    Manager.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    InTable.Navigate(new Page4());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    bek.Visibility = Visibility.Hidden;
                    Kassa.Visibility = Visibility.Hidden;
                    Manager.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        private void Kassa_Click(object sender, RoutedEventArgs e)
        {
            Window3 window3 = new Window3();
            window3.Show();
        }

        private void Manager_Click(object sender, RoutedEventArgs e)
        {
            Window2 window2 = new Window2();
            window2.Show();
        }
    }  
}
