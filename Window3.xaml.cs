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
    /// Логика взаимодействия для Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Spisok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Spisok.SelectedIndex)
            {
                case 0:
                    InTable.Navigate(new Page());
                    Exit.Visibility = Visibility.Visible;
                    Info.Visibility = Visibility.Visible;
                    break;
                case 1:
                    InTable.Navigate(new Page1_2());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    InTable.Navigate(new Page2_2());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    InTable.Navigate(new Page3_2());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    InTable.Navigate(new Page4_2());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    break;
                case 5:
                    InTable.Navigate(new Page5());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    break;
                case 6:
                    InTable.Navigate(new Page6());
                    Exit.Visibility = Visibility.Hidden;
                    Info.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

    }
}
