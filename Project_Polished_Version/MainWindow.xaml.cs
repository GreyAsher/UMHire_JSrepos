using MySql.Data.MySqlClient;
using Project_Polished_Version.Classes;
using System;
using System.Collections.Generic;
using System.Windows;


namespace Project_Polished_Version
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _connection = "Server=127.0.0.1;Database=project_database;UserID=root;Password=SQLDatabase404;";
        private readonly MySqlConnection connection;
        public static Dictionary<string, ApplicantUser> userAccounts;
        public static Dictionary<int, ApplicantUser> userAccountsGetID = new Dictionary<int, ApplicantUser>();
        public static Dictionary<int, CompanyUser> companyAccountsGetID = new Dictionary<int, CompanyUser>();
        public static Dictionary<string, CompanyUser> companyAccounts;
        public static int UserID;
        public static int CompanyID;
        public static string UserEmail;
        public MainWindow()
        {
            InitializeComponent();

            // Initialize database connection
            connection = new MySqlConnection(_connection);

            // Initialize account data stores
            userAccounts = new Dictionary<string, ApplicantUser>(StringComparer.OrdinalIgnoreCase);
            companyAccounts = new Dictionary<string, CompanyUser>(StringComparer.OrdinalIgnoreCase);
            LoadDataIntoMemory();
        }

        private void LoadDataIntoMemory()
        {
            try
            {
                connection.Open();

                // Load user data
                string userQuery = "SELECT * FROM applicant_accounts";
                using (MySqlCommand command = new MySqlCommand(userQuery, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string email = reader["email"] != DBNull.Value ? reader["email"].ToString().Trim() : null;

                        // Skip records with null or empty email
                        if (string.IsNullOrEmpty(email))
                        {
                            Console.WriteLine("Skipped user record: Email is null or empty.");
                            continue;
                        }

                        ApplicantUser userRecord = new ApplicantUser
                        {
                            Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                            First_Name = reader.IsDBNull(reader.GetOrdinal("first_name")) ? "" : reader.GetString("first_name").Trim(),
                            Last_Name = reader.IsDBNull(reader.GetOrdinal("last_name")) ? "" : reader.GetString("last_name").Trim(),
                            JobTitle = reader.IsDBNull(reader.GetOrdinal("Job_title")) ? "" : reader.GetString("Job_title").Trim(),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("Phone_Number")) ? "" : reader.GetString("Phone_Number").Trim(),
                            Gender = reader.IsDBNull(reader.GetOrdinal("gender")) ? "" : reader.GetString("gender").Trim(),
                            Address = reader["address"] != DBNull.Value ? reader["address"].ToString().Trim() : null,
                            Email = email,
                            Password = reader["password"] != DBNull.Value ? reader["password"].ToString().Trim() : null
                        };

                        // Add to dictionaries
                        userAccounts[email] = userRecord;
                        userAccountsGetID[userRecord.Id] = userRecord;
                    }
                }

                // Load company data
                string companyQuery = "SELECT * FROM company_accounts";
                using (MySqlCommand command = new MySqlCommand(companyQuery, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email").Trim();

                        //Skip records with null or empty email
                        if (string.IsNullOrEmpty(email))
                        {
                            Console.WriteLine("Skipped company record: Email is null or empty.");
                            continue;
                        }

                        CompanyUser companyRecord = new CompanyUser
                        {
                            CompanyId = reader.IsDBNull(reader.GetOrdinal("company_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("company_id")),
                            CompanyName = reader.IsDBNull(reader.GetOrdinal("company_name")) ? "" : reader.GetString("company_name").Trim(),
                            CompanyAddress = reader.IsDBNull(reader.GetOrdinal("company_address")) ? "" : reader.GetString("company_address").Trim(),
                            CompanyEmail = email,
                            CompanyPassword = reader.IsDBNull(reader.GetOrdinal("password")) ? "" : reader.GetString("password").Trim()
                        };

                        // Add to dictionaries
                        companyAccounts[email] = companyRecord;
                        companyAccountsGetID[companyRecord.CompanyId] = companyRecord;
                    }
                }
            }
            catch (Exception ex)
            {
                // Enhanced error message with stack trace for debugging
                MessageBox.Show($"Error loading data: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                connection.Close();
            }
        }


        private bool AuthenticateUser(string username, string password, out int id, out bool isCompany)
        {
            // Check user accounts
            if (userAccounts.TryGetValue(username, out ApplicantUser userRecord) && userRecord.Password == password)
            {
                id = userRecord.Id;
                isCompany = false;
                return true;
            }

            // Check company accounts
            if (companyAccounts.TryGetValue(username, out CompanyUser companyRecord) && companyRecord.CompanyPassword == password)
            {
                id = companyRecord.CompanyId;
                isCompany = true;
                return true;
            }

            id = 0;
            isCompany = false;
            return false;
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to register page
            new Register_Window().Show();
            //this.Hide();
        }

        private void Log_In_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sanitizedUsername = UsernameTextBox.Text.Trim();
                string sanitizedPassword = PasswordTxtBox.Password;

                if (AuthenticateUser(sanitizedUsername, sanitizedPassword, out int id, out bool isCompany))
                {
                    if (isCompany)
                    {
                        // Navigate to company dashboard
                        CompanyID = id;
                        new Company_DashBoard().Show();
                    }
                    else
                    {
                        // Navigate to user dashboard
                        UserEmail = sanitizedUsername;
                        UserID = id;
                        new Applicant_DashBoard().Show();
                    }
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Show: " + ex.Message);
            }
        }

        private void Register_Button_Click_Company(object sender, RoutedEventArgs e)
        {
            Register_Company rc = new Register_Company();
            this.Hide();
            rc.Show();
        }

        private void unhashed_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordTxtBox.Visibility == Visibility.Visible)
            {
                Password.Text = PasswordTxtBox.Password;
                Password.Visibility = Visibility.Visible;
                PasswordTxtBox.Visibility = Visibility.Collapsed;

                UnhashedPassword.Content = "Show Password";
            }
            else
            {
                PasswordTxtBox.Password = Password.Text;
                Password.Visibility = Visibility.Collapsed;
                PasswordTxtBox.Visibility = Visibility.Visible;

                UnhashedPassword.Content = "Hide Password";
            }
        }
    }
}

