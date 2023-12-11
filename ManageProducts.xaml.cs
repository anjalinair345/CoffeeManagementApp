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
using Microsoft.VisualBasic;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using System.Drawing;
using System.Security.Policy;

namespace CoffeeManagementApp
{
    /// <summary>
    /// Interaction logic for ManageProducts.xaml
    /// </summary>
    public partial class ManageProducts : Window
    {
        ManageEmployee managempl;
        Home home;
        MainWindow login;
        Report report;

        public ManageProducts(Home hme, MainWindow windw)
        {
            home = hme;
            login = windw;
            //  managempl = empl;
            InitializeComponent();
            WindowState = WindowState.Maximized; // Maximizes the window initially

            LoadProd();
        }
        SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True");

        public void LoadProd()
        {
            SqlCommand cmd = new SqlCommand("SELECT ProductID, ProductName, Price, Ingredients, Sizes FROM Products", connection);
            DataTable dt = new DataTable();
            connection.Open();
            SqlDataReader tableInfo = cmd.ExecuteReader();
            dt.Load(tableInfo);
            connection.Close();
            datagridprod.ItemsSource = dt.DefaultView;

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateProductFields())
            {

                string prodname = txtprod.Text;
                decimal price = (decimal)Math.Round(decimal.Parse(txtprice.Text), 2);
                string ingre = txtingre.Text;
                string size = cmbsize.Text;
                if (!string.IsNullOrEmpty(prodname))
                {
                    using (SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True")) ;
                    {
                        connection.Open();
                        string query = "Insert Into Products (ProductName,Price,Ingredients,Sizes) values(@ProductName,@Price,@Ingredients,@Sizes)";
                        SqlCommand command = new SqlCommand(query, connection);

                        command.Parameters.AddWithValue("@ProductName", prodname);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@Ingredients", ingre);
                        command.Parameters.AddWithValue("@Sizes", size);

                        command.ExecuteNonQuery();
                        Clearprod();
                        connection.Close();

                        LoadProd();


                    }
                }
                MessageBox.Show("Product added successfully!");

            }
        }
        public void Clearprod()
        {
            txtprod.Text = "";
            txtprice.Text = "";
            txtingre.Text = "";
            cmbsize.Text = "";
        }
    
        public void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (datagridprod.SelectedItem != null)
            {
                DataRowView dataRowView = (DataRowView)datagridprod.SelectedItem;
                string selectedProd= Convert.ToString(dataRowView.Row[0]);
                SqlCommand cmd = new SqlCommand("Delete from Products where ProductName='" + selectedProd + "'", connection);
                connection.Open();
                cmd.ExecuteReader();
                connection.Close();
                MessageBox.Show("Product deleted");
                DataTable dt = new DataTable();
                connection.Open();
                SqlDataReader tableInfo = cmd.ExecuteReader();
                dt.Load(tableInfo);
                connection.Close();
                LoadProd();
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
                Add.IsEnabled = false;
                update.IsEnabled = true;
                Button button = sender as Button;
                if (button != null)
                {
                    DependencyObject dep = button;

                    // Find the DataGridRow which contains the clicked button
                    while (!(dep is DataGridRow) && dep != null)
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }

                    if (dep is DataGridRow)
                    {
                        DataGridRow row = dep as DataGridRow;

                        if (row != null && row.Item is DataRowView)
                        {
                            DataRowView rowselected = row.Item as DataRowView;

                            // Access the cell values directly by column name

                          

                            txtprod.Text = rowselected["ProductName"].ToString();
                            txtprice.Text = rowselected["Price"].ToString();
                            txtingre.Text = rowselected["Ingredients"].ToString();
                            cmbsize.Text = rowselected["Sizes"].ToString();

                        }
                    }
                
            }
        }

            private bool ValidateProductFields()
            {
                // Check for empty fields or invalid input
                if (string.IsNullOrEmpty(txtprod.Text) || string.IsNullOrEmpty(txtprice.Text) ||
                    string.IsNullOrEmpty(txtingre.Text) || string.IsNullOrEmpty(cmbsize.Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return false; // Validation failed
                }

                // Validate price field
                decimal price;
                if (!decimal.TryParse(txtprice.Text, out price) || price <= 0)
                {
                    MessageBox.Show("Please enter a valid price.");
                    return false; // Validation failed
                }

                // Add more specific validations as needed

                return true; // Validation succeeded
            }




            private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
             managempl =new ManageEmployee(home,login);
            managempl.Show();

        }

        private void HomeMenu(object sender, RoutedEventArgs e)
        {
            home=new Home(login);
            home.Show();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            login=new MainWindow();
            login.Show();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Clearprod();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            report=new Report(home,login);

            report.Show();
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (ValidateProductFields())
            {
                if (datagridprod.SelectedItem != null)
                {
                    DataRowView row = (DataRowView)datagridprod.SelectedItem;


                   int prodid = Convert.ToInt32(row["ProductID"]);
                   string prodname = txtprod.Text;
                    decimal price = (decimal)Math.Round(decimal.Parse(txtprice.Text), 2);
                    string ingre = txtingre.Text;
                    string size = cmbsize.Text;
                    // Retrieve other field values as needed
                    string updatequery = "UPDATE Products SET ProductName=@ProductName,Price = @Price, Ingredients = @Ingredients, Sizes = @Sizes WHERE ProductID = @ProductID";

                    try
                    {

                        using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-5614LLFD\\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True"))
                        {


                            SqlCommand command = new SqlCommand(updatequery, connection);
                            command.Parameters.AddWithValue("@ProductID", prodid);
                            command.Parameters.AddWithValue("ProductName", prodname);
                            command.Parameters.AddWithValue("@Price", price);
                            command.Parameters.AddWithValue("@Ingredients", ingre);
                            command.Parameters.AddWithValue("@Sizes", size);



                            connection.Open();

                            // Execute the query
                            int rowsAffected = command.ExecuteNonQuery();
                            Clearprod();
                            
                            Add.IsEnabled = true;
                            update.IsEnabled = false;

                            // Check if the query affected any rows
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Product Details Updated Successfully");
                                connection.Close();
                                LoadProd();
                            }
                            else
                            {
                                MessageBox.Show("Product Details not updated");

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error for updating" + ex.Message);
                        connection.Close();
                    }
                }
            }
        }
    }
}
