using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Linq;

namespace Project_Polished_Version
{
    /// <summary>
    /// Interaction logic for Register_Company.xaml
    /// </summary>
    public partial class Register_Company : Window
    {
        public Register_Company()
        {
            InitializeComponent();
        }

        private void Next_btn(object sender, RoutedEventArgs e)
        {
            // Retrieve input values from the form
            string companyName = Company_Name_TxtBox.Text.Trim();
            string companyEmail = Company_Email_TxtBox.Text.Trim();
            string companyMobileNumber = Company_PassWord_txtBox.Password;
            string companyAddress = Address_TxtBox.Text.Trim();

            // Validation
            if (string.IsNullOrWhiteSpace(companyName) ||
                string.IsNullOrWhiteSpace(companyEmail) ||
                string.IsNullOrWhiteSpace(companyMobileNumber) ||
                string.IsNullOrWhiteSpace(companyAddress))
            {
                MessageBox.Show("All fields are required. Please fill out all the information.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (companyMobileNumber.Any(c => !char.IsDigit(c)))
            {
                MessageBox.Show("The mobile number must contain only digits.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (companyEmail.Length < 8)
            {
                MessageBox.Show("The password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Database connection string
            string connectionString = "Server=localhost;Database=project_database;UserID=root;Password=Cedric1234%%;";

            // SQL insert query
            string query = "INSERT INTO company_accounts (Company_Name, Company_address, password, email) " +
                           "VALUES (@Company_Name, @Company_address, @password, @Email)";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create a SQL command
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Company_Name", companyName);
                        command.Parameters.AddWithValue("@Company_address", companyAddress);
                        command.Parameters.AddWithValue("@password", companyMobileNumber);
                        command.Parameters.AddWithValue("@Email", companyEmail);

                        // Execute the command
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            AddCompanyInfo();
                        }
                        else
                        {
                            MessageBox.Show("Registration failed. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCompanyInfo()
        {
            string connectionString = "Server=localhost;Database=project_database;UserID=root;Password=Cedric1234%%;";
            string insertQuery = "INSERT INTO applicant_info (userId, info_type, date_started, date_ended, content) VALUES (@userId, @info_type, @date_started, @date_ended, @content)";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@userId", MainWindow.CompanyID);
                        command.Parameters.AddWithValue("@info_type", "About_Post");
                        command.Parameters.AddWithValue("@date_started", DateTime.Now);
                        command.Parameters.AddWithValue("@date_ended", DateTime.Now);
                        command.Parameters.AddWithValue("@content", DBNull.Value);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Company info added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting company info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
