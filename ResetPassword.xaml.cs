using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace CoffeeManagementApp
{
    /// <summary>
    /// Interaction logic for ResetPassword.xaml
    /// </summary>
    public partial class ResetPassword : Window
    {
        MainWindow login;

        public ResetPassword()
        {
            InitializeComponent();
            login = new MainWindow();
            WindowState = WindowState.Maximized; // Maximizes the window initially
        }

        //Update Password
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = newpassword.Password;
            string confirmPassword = confirmpassword.Password;
            string userName = username.Text; // Assuming the username is entered in a TextBox named 'usernameTextBox'

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please re-enter.");
                return;
            }

            string connectionString = "Data Source=LAPTOP-5614LLFD\\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True"; // Replace with your actual connection string

            // Update password query
            string updatePasswordQuery = "UPDATE UserLogin SET Password = @Password WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(updatePasswordQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Password", newPassword);
                        command.Parameters.AddWithValue("@Username", userName);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password updated successfully.");
                            // Clear the text boxes or take any other necessary action
                            login.Show();
                        }
                        else
                        {
                            MessageBox.Show("Username not found. Password update failed.");
                        }

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    connection.Close();
                }
            }
        }
    }
    
}
