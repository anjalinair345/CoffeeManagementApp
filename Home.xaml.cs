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

namespace CoffeeManagementApp
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        ManageEmployee manageemp;
        ManageProducts manageprod;
        MainWindow login;
        Report rep;
      //  ManageProducts manageprod = new ManageProducts();
        // Home home = new Home();
        public Home(MainWindow windw)
        {
            login=windw;

            InitializeComponent();
            manageemp = new ManageEmployee(this,login);
            manageprod=new ManageProducts(this,login);
            rep = new Report(this, login);
            WindowState = WindowState.Maximized; // Maximizes the window initially


        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
           manageemp.Show();
        }

        private void Reports(object sender, RoutedEventArgs e)
        {
            rep.Show();
        }

        private void Prod_click(object sender, RoutedEventArgs e)
        {
            manageprod.Show();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            login = new MainWindow();
            login.Show();
        }
    }
}
