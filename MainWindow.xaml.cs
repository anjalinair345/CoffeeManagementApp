using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace CoffeeManagementApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Home home;
        ResetPassword resetpswd;
        PlaceOrder order;

        public MainWindow()
        {
            InitializeComponent();
            home = new Home(this);
            order = new PlaceOrder(this);
            WindowState = WindowState.Maximized; // Maximizes the window initially

        }
        SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True");

        //Login Click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = uname.Text;
            string pswd = password.Password;
            string employerusername = "Admin";
            string employerpassword = "Admin123";
            if (Employer.IsChecked == true)
            {

                if ((employerusername == username) && (employerpassword == pswd))
                {
                    home.Show();

                }
                else
                {
                    MessageBox.Show("Invalid username or password.");

                }
            }
            else if (Employee.IsChecked == true)
            {
                if (AuthenticateUser(username, pswd))
                {
                    if (username == pswd)
                    {
                        // Redirect to ResetPassword page
                        resetpswd = new ResetPassword();

                        resetpswd.Show();
                    }
                    else
                    {
                        // Redirect to another page or perform further actions here

                        order.Show();
                    }
                }

                else
                {
                    MessageBox.Show("Invalid username or password.");

                }
            }
            else
            {
                MessageBox.Show("Select User Type!");

            }

            connection.Close();

        }

        private bool AuthenticateUser(string username, string pswd)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True"))

            {
                string query = "SELECT COUNT(*) FROM UserLogin WHERE UserName = @Username AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", pswd);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void ForgotPasswordLabel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            resetpswd = new ResetPassword();

            resetpswd.Show();
        }
    }
}