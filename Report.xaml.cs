using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        Home home;
        ManageEmployee emp;
        MainWindow login;
        ManageProducts prod;

        public Report(Home hme,MainWindow windw)
        {
            InitializeComponent();
            login = windw;
            home = hme;
            emp=new ManageEmployee(hme,windw);
            prod=new ManageProducts(hme,windw);
            WindowState = WindowState.Maximized; // Maximizes the window initially

        }

        SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True");
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            if (selectedItem != null)
            {
                string selectedOption = selectedItem.Content?.ToString();
                option1Fields.Visibility = (selectedOption == "Employees") ? Visibility.Visible : Visibility.Collapsed;
                option2Fields.Visibility = (selectedOption == "Order") ? Visibility.Visible : Visibility.Collapsed;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)

        {
            if (ComboBox.SelectedItem != null)
            {
                string selectedOption = ComboBox.Text;

                string query = "";
                DateTime dateofjoining = MyDatePicker.SelectedDate.GetValueOrDefault();
                string given = ordertype.Text;
                switch (selectedOption)
                {
                    case "Employees":
                        query = "SELECT * FROM Employee WHERE Hiredate > @dateofjoining";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@dateofjoining", dateofjoining);

                            connection.Open();


                            DataTable dt = new DataTable();
                            SqlDataReader tableInfo = cmd.ExecuteReader();
                            dt.Load(tableInfo);

                            connection.Close();


                            datagrid.ItemsSource = dt.DefaultView;

                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("No data found.");
                            }
                        }

                        break;
                    case "Order":
                        if (given == "Cash")
                        {
                            query = "SELECT * FROM Orders WHERE PaymentType = 'Cash'";
                        }
                        else
                        {
                            query = "SELECT * FROM Orders WHERE PaymentType = 'Card'";
                        }

                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {


                            connection.Open();


                            DataTable dt = new DataTable();
                            SqlDataReader tableInfo = cmd.ExecuteReader();
                            dt.Load(tableInfo);

                            connection.Close();


                            datagrid.ItemsSource = dt.DefaultView;

                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("No data found.");
                            }
                        }
                        break;
                    default:
                        break;
                }

            }

            else
            {
                MessageBox.Show("Select any field");
            }
            Clear();
        }
        public void Clear()
        {
            ComboBox.Text = "";
            MyDatePicker.SelectedDate = null;
            ordertype.Text = "";


        }
        private void Button_ClickHome(object sender, RoutedEventArgs e)
        {
            home = new Home(login);
            home.Show();
        }
        private void Button_ClickEmployee(object sender, RoutedEventArgs e)
        {

            emp.Show();
            Close();
        }
        private void Button_ClickProducts(object sender, RoutedEventArgs e)
        {
            prod.Show();
            Close();
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            login = new MainWindow();
            login.Show();
        }
    }
}
