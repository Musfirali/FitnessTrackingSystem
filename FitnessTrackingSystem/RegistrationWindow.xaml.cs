using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows;
using System.Data.SqlClient;
using System.Data;

namespace FitnessTrackingSystem
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text;
            string ageText = ageTextBox.Text;
            string gender = genderTextBox.Text.ToUpper();
            string heightText = heightTextBox.Text;
            string weightText = weightTextBox.Text;
            string email = emailTextBox.Text;
            string password = passwordBox.Password;

            // Validate inputs
            if (IsValidName(name) && IsValidAge(ageText) && IsValidGender(gender) && IsValidHeight(heightText) && IsValidWeight(weightText) && IsValidEmail(email) && IsValidPassword(password))
            {
                // Convert text to appropriate data types
                int age = int.Parse(ageText);
                decimal height = decimal.Parse(heightText);
                decimal weight = decimal.Parse(weightText);

                // Insert into database
                string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "EXEC InsertCustomer @Email, @Password, @Name, @Gender, @Age, @Height, @Weight";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Height", height);
                    command.Parameters.AddWithValue("@Weight", weight);

                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                        Console.WriteLine("Connection opened successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Connection already open.");
                    }

                    //connection.Open();
                    int result = command.ExecuteNonQuery();

                    // Check if the insertion was successful
                    if (result > 0)
                    {
                        Console.WriteLine("Registration successful!");
                        MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Console.WriteLine("Registration failed.");
                        MessageBox.Show("Registration failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please fill out all fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private bool IsValidName(string name) => !string.IsNullOrWhiteSpace(name);
        private bool IsValidAge(string ageText) => int.TryParse(ageText, out int age) && age > 0;
        private bool IsValidGender(string gender) => gender == "M" || gender == "F" || gender == "O";
        private bool IsValidHeight(string heightText) => decimal.TryParse(heightText, out decimal height) && height > 0;
        private bool IsValidWeight(string weightText) => decimal.TryParse(weightText, out decimal weight) && weight > 0;

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

        private bool IsValidPassword(string password) => password.Length >= 6;

        private void nameTextBox_TextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void nameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            nameTextBox.Text.Remove(0);
        }
    }
}
