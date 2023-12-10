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

namespace CoffeeManagementApp
{
    /// <summary>
    /// Interaction logic for PlaceOrder.xaml
    /// </summary>
    public partial class PlaceOrder : Window
    {
        MainWindow login;
        public PlaceOrder(MainWindow window)
        {
            InitializeComponent();

            WindowState = WindowState.Maximized; // Maximizes the window initially

            login = window;
            Loaddata();
            // Fetch and bind coffee types to cmbCoffeeTypes
            List<string> coffeeTypes = FetchCoffeeTypesFromDatabase();
            type.ItemsSource = coffeeTypes;


            // Fetch and bind coffee sizes to cmbCoffeeSizes

            type.SelectionChanged += cmbCoffeeTypes_SelectionChanged;
            size.SelectionChanged += CmbCoffee_SelectionChanged;


        }
        SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True");

        private void Loaddata()
        {

            SqlCommand cmd = new SqlCommand("SELECT BillNumber, CustomerName, Price, Quantity, TotalPrice, CoffeeType, CoffeeSize, PaymentType FROM Orders", connection);
            DataTable dt = new DataTable();
            connection.Open();
            SqlDataReader tableInfo = cmd.ExecuteReader();
            dt.Load(tableInfo);
            connection.Close();
            datgridplace.ItemsSource = dt.DefaultView;
        }





        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ValidateOrderFields())
            {
                int billno = Convert.ToInt32(txtbillno.Text);
                string customer_name = txtcustomer_name.Text;
                decimal price = (decimal)Math.Round(decimal.Parse(txtprice.Text), 2);
                int quantity = Convert.ToInt32(txtquantity.Text);
                decimal totalprice = Convert.ToDecimal(txttotalprice.Text);
                string coffeetype = type.Text;
                string coffeesize = size.Text;
                string paymenttype = payment.Text;
                if (!string.IsNullOrEmpty(txtbillno.Text))
                {
                    using (SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True")) ;
                    {
                        connection.Open();
                        string query = "Insert Into Orders (BillNumber,CustomerName,CoffeeType,CoffeeSize,Price,Quantity,TotalPrice,PaymentType) values(@BillNumber,@CustomerName,@CoffeeType,@CoffeeSize,@Price,@Quantity,@TotalPrice,@PaymentType)";
                        SqlCommand command = new SqlCommand(query, connection);

                        command.Parameters.AddWithValue("@BillNumber", billno);
                        command.Parameters.AddWithValue("@CustomerName", customer_name);
                        command.Parameters.AddWithValue("@CoffeeType", coffeetype);
                        command.Parameters.AddWithValue("@CoffeeSize", coffeesize);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@TotalPrice", totalprice);
                        command.Parameters.AddWithValue("@PaymentType", paymenttype);

                        command.ExecuteNonQuery();

                        SqlCommand cmd = new SqlCommand("Select * from Orders", connection);
                        DataTable dt = new DataTable();
                        //connection.Open();
                        SqlDataReader tableInfo = cmd.ExecuteReader();
                        dt.Load(tableInfo);
                        connection.Close();
                        datgridplace.ItemsSource = dt.DefaultView;


                    }
                }
                MessageBox.Show("Order Placed successfully!");
                ClearFields();

            }
        }

        private void cmbCoffeeTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCoffeeType = type.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedCoffeeType))
            {
                List<string> sizes = FetchCoffeeSizesByTypeFromDatabase(selectedCoffeeType);

                size.ItemsSource = sizes;
            }
        }

        // Fetch distinct coffee types from the database
        private List<string> FetchCoffeeTypesFromDatabase()
        {
            List<string> types = new List<string>();

            string connectionString = @"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True";
            string query = "SELECT DISTINCT ProductName FROM Products";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string coffeeType = reader["ProductName"].ToString();
                        types.Add(coffeeType);
                    }
                }
            }

            return types;
        }

        // Fetch coffee sizes based on selected coffee type from the database
        private List<string> FetchCoffeeSizesByTypeFromDatabase(string selectedCoffeeType)
        {
            List<string> sizes = new List<string>();

            string connectionString = @"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True";
            string query = "SELECT DISTINCT Sizes FROM Products WHERE ProductName = @ProductName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductName", selectedCoffeeType);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string coffeeSize = reader["Sizes"].ToString();
                        sizes.Add(coffeeSize);
                    }
                }
            }

            return sizes;
        }


        private void CmbCoffee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCoffeeType = type.SelectedItem as string;
            string selectedCoffeeSize = size.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedCoffeeType) && !string.IsNullOrEmpty(selectedCoffeeSize))
            {
                string connectionString = @"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True";
                string query = "SELECT Price FROM Products WHERE ProductName = @ProductName AND Sizes = @Sizes";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand getPriceCommand = new SqlCommand(query, connection);
                    getPriceCommand.Parameters.AddWithValue("@ProductName", selectedCoffeeType);
                    getPriceCommand.Parameters.AddWithValue("@Sizes", selectedCoffeeSize);

                    // Execute the query to get the price based on coffee type and size
                    object result = getPriceCommand.ExecuteScalar();
                    if (result != null)
                    {
                        //  float price = Convert.ToSingle(result); // Convert to float
                        decimal price = Convert.ToDecimal(result);

                        // Display the fetched price in the txtprice field or any other TextBox
                        txtprice.Text = price.ToString(); // Update txtprice with the fetched value
                    }
                    else
                    {
                        // Handle case where coffee type and size combination is not found in the database
                        MessageBox.Show("Combination of coffee type and size not found in the database.");
                        txtprice.Text = string.Empty; // Clear the field if no price is found
                    }
                }
            }
        }
        private bool ValidateOrderFields()
        {
            int billno;
            if (!int.TryParse(txtbillno.Text, out billno) || string.IsNullOrWhiteSpace(txtcustomer_name.Text) ||
                !decimal.TryParse(txtprice.Text, out decimal price) || !int.TryParse(txtquantity.Text, out int quantity) ||
                !decimal.TryParse(txttotalprice.Text, out decimal totalprice) || string.IsNullOrWhiteSpace(type.Text) ||
                string.IsNullOrWhiteSpace(size.Text) || string.IsNullOrWhiteSpace(payment.Text))
            {
                MessageBox.Show("Please fill in all fields with valid data.");
                return false; // Validation failed
            }

            // Add more specific validations as needed

            return true; // Validation succeeded
        }

        private void ClearFields()
        {
            txtbillno.Text = string.Empty;
            txtcustomer_name.Text = string.Empty;
            txtprice.Text = string.Empty;
            txtquantity.Text = string.Empty;
            txttotalprice.Text = string.Empty;
            size.SelectedIndex = -1;
            type.SelectedIndex = -1;
            payment.SelectedIndex = -1;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (datgridplace.SelectedItem != null)
            {
                DataRowView dataRowView = (DataRowView)datgridplace.SelectedItem;
                int billno = Convert.ToInt32(dataRowView.Row[0]);
                SqlCommand cmd = new SqlCommand("Delete from Orders where OrderID='" + billno + "'", connection);
                connection.Open();
                cmd.ExecuteReader();
                connection.Close();
                MessageBox.Show("Product deleted");
                DataTable dt = new DataTable();
                connection.Open();
                SqlDataReader tableInfo = cmd.ExecuteReader();
                dt.Load(tableInfo);
                connection.Close();

            }
            Loaddata();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Add.IsEnabled = false;
            Update.IsEnabled = true;
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


                        txtbillno.Text = rowselected["BillNumber"].ToString();
                        txtcustomer_name.Text = rowselected["CustomerName"].ToString();
                        txtprice.Text = rowselected["Price"].ToString();
                        txtquantity.Text = rowselected["Quantity"].ToString();
                        txttotalprice.Text = rowselected["TotalPrice"].ToString();
                        type.Text = rowselected["CoffeeType"].ToString();
                        size.Text = rowselected["CoffeeSize"].ToString();
                        payment.Text = rowselected["PaymentType"].ToString();

                    }
                }
            }
        }
        private void txtquantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotalPrice();
        }

        private void txtprice_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            if (int.TryParse(txtquantity.Text, out int quantity) && float.TryParse(txtprice.Text, out float price))
            {
                float totalprice = quantity * price;
                txttotalprice.Text = totalprice.ToString();
            }
        }

        private void Button_logout(object sender, RoutedEventArgs e)
        {
            login = new MainWindow();
            login.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ValidateOrderFields())
            {
                if (datgridplace.SelectedItem != null)
                {
                    DataRowView row = (DataRowView)datgridplace.SelectedItem;

                    int orderId = Convert.ToInt32(row["OrderID"]);

                    int billno = Convert.ToInt32(txtbillno.Text);
                    string customer_name = txtcustomer_name.Text;
                    decimal price = Convert.ToDecimal(txtprice.Text);
                    int quantity = Convert.ToInt32(txtquantity.Text);
                    decimal totalprice = Convert.ToDecimal(txttotalprice.Text);
                    string coffeetype = type.Text;
                    string coffeesize = size.Text;
                    string paymenttype = payment.Text;
                    // Retrieve other field values as needed
                    string query = "UPDATE Orders SET BillNumber = @BillNumber, CustomerName = @CustomerName, Price = @Price, Quantity = @Quantity, TotalPrice = @TotalPrice, CoffeeType = @CoffeeType, CoffeeSize = @CoffeeSize, PaymentType = @PaymentType WHERE OrderID = @OrderID";

                    try
                    {

                        using (SqlConnection connection = new SqlConnection("Data Source=LAPTOP-5614LLFD\\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True"))
                     {

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        command.Parameters.AddWithValue("@BillNumber", billno);
                        command.Parameters.AddWithValue("@CustomerName", customer_name);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@TotalPrice", totalprice);
                        command.Parameters.AddWithValue("@CoffeeType", coffeetype);
                        command.Parameters.AddWithValue("@CoffeeSize", coffeesize);
                        command.Parameters.AddWithValue("@PaymentType", paymenttype);

                            connection.Open();

                            // Execute the query
                            int rowsAffected = command.ExecuteNonQuery();
                            placeOrderClear();
                            Add.IsEnabled = true;
                            Update.IsEnabled = false;


                            // Check if the query affected any rows
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Place Order Details Updated Successfully");
                                connection.Close();
                                Loaddata();
                            }
                            else
                            {
                                MessageBox.Show("Order Details not updated");

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
        private void placeOrderClear()
        {
            txtbillno.Text = string.Empty;
            txtcustomer_name.Text = string.Empty;
            txtprice.Text = string.Empty;
            txtquantity.Text = string.Empty;
            txttotalprice.Text = string.Empty;
            type.Text = string.Empty;
            size.Text = string.Empty;
            payment.Text = string.Empty;
        }
    }
    }
