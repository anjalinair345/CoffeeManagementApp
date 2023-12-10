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
using System.Globalization;

namespace CoffeeManagementApp
{
    /// <summary>
    /// Interaction logic for ManageEmployee.xaml
    /// </summary>
    public partial class ManageEmployee : Window
    {
        MainWindow login;
        Home home;
        ManageProducts manageprod;
        Report rep;

        public ManageEmployee(Home hme, MainWindow windw)
        {
            home = hme;
            login = windw;
            InitializeComponent();
            Load();
            WindowState = WindowState.Maximized; // Maximizes the window initially

        }

        SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-5614LLFD\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True");

        //public void Load()
        //{
        //    DataTable dt = new DataTable();
        //    var emplist = GetEmployees();
        //    connection.Close();
        //    DatagridEmp.ItemsSource = emplist;

        //}

        public void Load()
        {
            // SqlCommand cmd = new SqlCommand("Select * from Employees", connection);
            SqlCommand cmd = new SqlCommand("Select EmployeeID,FirstName,LastName,DOB,EmployeeType,Email,Gender,SinNumber,SinExpiry,Contact,Designation,HireDate,Address from Employee", connection);

            DataTable dt = new DataTable();
            connection.Open();
            SqlDataReader tableInfo = cmd.ExecuteReader();
            dt.Load(tableInfo);


            connection.Close();

            DatagridEmp.ItemsSource = dt.DefaultView;

        }
       
        private void Save(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                string firstname = fname.Text;
                string lastname = lname.Text;
                DateTime dateofjoining = doj.SelectedDate.GetValueOrDefault().Date;
                string employeetype = emptype.Text;
                DateTime dateofbirth = dob.SelectedDate.GetValueOrDefault().Date;
                string emailaddress = emailadd.Text;
                string gendertype = gender.Text;
                string contactnum = contact.Text;
                string sinnumber = sinno.Text;
                DateTime sinexpiry = sinexp.SelectedDate.GetValueOrDefault().Date;
                string designation = design.Text;
                string empaddress = address.Text;

                string connectionString = "Data Source=LAPTOP-5614LLFD\\SQLEXPRESS;Initial Catalog=CoffeeShop;Integrated Security=True"; // Replace with your actual connection string

                string generatedEmpID = GenerateNextEmpID(connectionString);

                string query = "INSERT INTO Employee(EmployeeID,FirstName, LastName, DOB, EmployeeType, Email, Gender, SinNumber, SinExpiry, Contact, Designation, HireDate, Address)" +
                                "VALUES (@EmployeeID,@FirstName, @LastName, @DateOfBirth, @EmployeeType, @Email, @Gender, @SinNumber, @SinExpiry, @ContactNumber, @Designation, @DateofJoining, @Address)";
                string insertUserQuery = "INSERT INTO UserLogin(Username, Password) VALUES (@Username, @Password)";
                SqlConnection connection = new SqlConnection(connectionString);

                try
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        using (SqlCommand command = new SqlCommand(query, connection, transaction))
                        {

                            command.Parameters.AddWithValue("@EmployeeID", generatedEmpID); // Insert generated EMPID
                            command.Parameters.AddWithValue("@FirstName", firstname);
                            command.Parameters.AddWithValue("@LastName", lastname);
                            command.Parameters.AddWithValue("DateOfBirth", dateofbirth);
                            command.Parameters.AddWithValue("@EmployeeType", employeetype);
                            command.Parameters.AddWithValue("@Email", emailaddress);
                            command.Parameters.AddWithValue("@Gender", gendertype);
                            command.Parameters.AddWithValue("@SinNumber", sinnumber);
                            command.Parameters.AddWithValue("@SinExpiry", sinexpiry);
                            command.Parameters.AddWithValue("@ContactNumber", contactnum);
                            command.Parameters.AddWithValue("@Designation", designation);
                            command.Parameters.AddWithValue("@DateOfJoining", dateofjoining);
                            command.Parameters.AddWithValue("@Address", empaddress);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Insert into UserLogin table
                                string username = generatedEmpID; //Default username
                                string password = generatedEmpID; // Default password

                                using (SqlCommand insertUserCommand = new SqlCommand(insertUserQuery, connection, transaction))
                                {
                                    insertUserCommand.Parameters.AddWithValue("@Username", username);
                                    insertUserCommand.Parameters.AddWithValue("@Password", password);

                                    int userRowsAffected = insertUserCommand.ExecuteNonQuery();

                                    if (userRowsAffected > 0)
                                    {
                                        MessageBox.Show($"Employee and login credentials inserted successfully.");
                                        transaction.Commit();
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Failed to insert login credentials for user {username}.");
                                        transaction.Rollback();
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Failed to insert employee details.");
                                transaction.Rollback();
                            }
                        }
                    }

                    connection.Close();
                    Clear();
                    Load();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    connection.Close();
                }
            }
        }
        private bool ValidateFields()
        {
            // Check for empty fields
            if (string.IsNullOrEmpty(fname.Text) || string.IsNullOrEmpty(lname.Text) ||
                string.IsNullOrEmpty(emptype.Text) || string.IsNullOrEmpty(emailadd.Text) ||
                string.IsNullOrEmpty(gender.Text) || string.IsNullOrEmpty(contact.Text) ||
                string.IsNullOrEmpty(sinno.Text) || string.IsNullOrEmpty(design.Text) ||
                string.IsNullOrEmpty(address.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return false; // Validation failed
            }

            // Check for valid dates
            if (doj.SelectedDate == null || dob.SelectedDate == null || sinexp.SelectedDate == null)
            {
                MessageBox.Show("Please select valid dates for date fields.");
                return false; // Validation failed
            }

            // Add more specific validations as needed

            return true; // Validation succeeded
        }


        public void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DatagridEmp.SelectedItem != null)
            {
                DataRowView dataRowView = (DataRowView)DatagridEmp.SelectedItem;
                String selectedPerson = Convert.ToString(dataRowView.Row[0]);
                SqlCommand cmd = new SqlCommand("Delete from Employee where EmployeeID='" + selectedPerson + "'", connection);
                connection.Open();
                cmd.ExecuteReader();
                connection.Close();
                MessageBox.Show("Employee deleted");
                DataTable dt = new DataTable();
                connection.Open();
                SqlDataReader tableInfo = cmd.ExecuteReader();
                dt.Load(tableInfo);
                connection.Close();
                Load();
            }
        }

        //Clear all fields
        public void Clear()
        {
            fname.Text = "";
            lname.Text = "";
            doj.SelectedDate = null;
            emptype.Text = "";
            dob.SelectedDate = null;
            emailadd.Text = "";
            gender.Text = "";
            contact.Text = "";
            sinno.Text = "";
            sinexp.SelectedDate = null;
            design.Text = "";
            address.Text = "";

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            dob.IsEnabled = false;
            doj.IsEnabled = false;
            save.IsEnabled = false;
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

                        fname.Text = rowselected["FirstName"].ToString();
                        lname.Text = rowselected["LastName"].ToString();
                        doj.Text = rowselected["HireDate"].ToString();
                        emptype.Text = rowselected["EmployeeType"].ToString();
                        dob.Text = rowselected["DOB"].ToString();
                        emailadd.Text = rowselected["Email"].ToString();
                        gender.Text = rowselected["Gender"].ToString();
                        contact.Text = rowselected["Contact"].ToString();
                        sinno.Text = rowselected["SinNumber"].ToString();
                        sinexp.Text = rowselected["SinExpiry"].ToString();
                        design.Text = rowselected["Designation"].ToString();
                        address.Text = rowselected["Address"].ToString();
                    }
                }
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                if (DatagridEmp.SelectedItem != null)
                {
                    DataRowView row = (DataRowView)DatagridEmp.SelectedItem;

                    // Get the EmployeeID from the selected row
                    //int employeeID = Convert.ToInt32(row["EmployeeID"]);
                    string employeeID = Convert.ToString(row["EmployeeID"]);
                    string firstname = fname.Text;
                    string lastname = lname.Text;
                    DateTime dateofjoining = doj.SelectedDate.GetValueOrDefault();
                    doj.IsEnabled = false;
                    string employeetype = emptype.Text;
                    DateTime dateofbirth = dob.SelectedDate.GetValueOrDefault();
                    dob.IsEnabled = false;
                    string emailaddress = emailadd.Text;
                    string gendertype = gender.Text;
                    string contactnum = contact.Text;
                    string sinnumber = sinno.Text;
                    DateTime sinexpiry = sinexp.SelectedDate.GetValueOrDefault();
                    string designation = design.Text;
                    string empaddress = address.Text;

                    // Your connection string

                    // Create an SQL query with parameters to update FirstName and LastName
                    string updateQuery = "UPDATE Employee SET FirstName=@FirstName,LastName=@LastName,DOB=@DOB,EmployeeType=@EmployeeType,Email=@Email,Gender=@Gender,SinNumber=@SinNumber,SinExpiry=@SinExpiry,Contact=@Contact,Designation=@Designation,HireDate=@HireDate,Address=@Address WHERE EmployeeID = @EmployeeID";

                    try
                    {
                        // Create a SqlCommand
                        using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                        {
                            // Add parameters

                            cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                            cmd.Parameters.AddWithValue("@FirstName", firstname);
                            cmd.Parameters.AddWithValue("@LastName", lastname);
                            cmd.Parameters.AddWithValue("DOB", dateofbirth);
                            cmd.Parameters.AddWithValue("@EmployeeType", employeetype);
                            cmd.Parameters.AddWithValue("@Email", emailaddress);
                            cmd.Parameters.AddWithValue("@Gender", gendertype);
                            cmd.Parameters.AddWithValue("@SinNumber", sinnumber);
                            cmd.Parameters.AddWithValue("@SinExpiry", sinexpiry);
                            cmd.Parameters.AddWithValue("@Contact", contactnum);
                            cmd.Parameters.AddWithValue("@Designation", designation);
                            cmd.Parameters.AddWithValue("@HireDate", dateofjoining);
                            cmd.Parameters.AddWithValue("@Address", empaddress);

                            // Open the connection
                            connection.Open();

                            // Execute the query
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Clear();
                            dob.IsEnabled = true;
                            doj.IsEnabled = true;
                            save.IsEnabled = true;
                            update.IsEnabled = false;

                            // Check if the query affected any rows
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Employee Details Updated Successfully");
                                connection.Close();
                                Load();
                            }
                            else
                            {
                                MessageBox.Show("Employee Details not updated");

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

        // Function to generate the next EMPID from the database
        private string GenerateNextEmpID(string connectionString)
        {
            string nextEmpID = "EMP001"; // Default EMPID if no employees exist

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Employee";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        int employeeCount = Convert.ToInt32(command.ExecuteScalar());

                        if (employeeCount == 0) // If no employees exist
                        {
                            // Insert the first employee with EMP1
                            nextEmpID = "EMP";
                        }
                        else
                        {
                            query = "SELECT ISNULL(MAX(SUBSTRING(EmployeeID, 4, LEN(EmployeeID))), '0') FROM Employee";

                            command.CommandText = query;
                            object result = command.ExecuteScalar();

                            int number;
                            if (int.TryParse(result.ToString(), out number))
                            {
                                number++; // Increment the number for the new EMPID
                                nextEmpID = "EMP" + number.ToString().PadLeft(3, '0'); // Adjust padding as needed
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }

            return nextEmpID;
        }

        //Create Login Credentials

        // Replace with your connection string
        
    private void Button_Click(object sender, RoutedEventArgs e)
        {
            login=new MainWindow();
            login.Show();


        }

        private void HomeMenu(object sender, RoutedEventArgs e)
        {
            home = new Home(login);
            home.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
          manageprod = new ManageProducts(home,login);

            manageprod.Show();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            rep=new Report(home,login); 
            rep.Show();
        }

       
    }
}


