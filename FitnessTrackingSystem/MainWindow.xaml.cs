using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
public sealed class Trace;

namespace FitnessTrackingSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text;
            string password = passwordBox.Password;
            bool isValidUser;
            int a;
            // Validate email and password
            if (IsValidEmail(email) && !string.IsNullOrEmpty(password))
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC ValidateUser @Email, @Password";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                        Console.WriteLine("Connection opened successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Connection already open.");
                    }

                    SqlParameter returnParameter = command.Parameters.Add("@UserIsValid", SqlDbType.Bit);
                    returnParameter.Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    object result = returnParameter.Value;  // Get the output value
                    if (result == DBNull.Value)
                    {
                        // Handle the case where no data is returned (e.g., invalid email)
                        isValidUser = false;
                        MessageBox.Show("Invalid email or database error.");
                    }
                    else
                    {
                        isValidUser = (bool)result;  // Cast to bool only if data is present
                    }

                    if (isValidUser == true)
                    {
                        MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Incorrect username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid email and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
