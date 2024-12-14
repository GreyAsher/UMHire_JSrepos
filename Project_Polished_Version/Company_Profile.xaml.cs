using MySql.Data.MySqlClient;
using Project_Polished_Version.Classes;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project_Polished_Version
{
    public partial class Company_Profile : Window
    {
        private string _connection = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%";
        public static int WindowNumber;
        public Company_Profile(CompanyUser company)
        {
            InitializeComponent();
            Company_Name.Text = company.CompanyName;
            Company_Email.Text = company.CompanyEmail;
        }

        public Company_Profile()
        {
            InitializeComponent();
            if (MainWindow.CompanyID <= 0)
            {
                MessageBox.Show("Invalid company ID. Cannot load profile.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }
            this.Loaded += OnWindowLoaded;
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            await LoadAboutDataAsync();
            ShowInfo();
        }

        private void ShowInfo()
        {
            int key = MainWindow.CompanyID;
            if (key <= 0 || !MainWindow.companyAccountsGetID.TryGetValue(key, out CompanyUser loggedUser))
            {
                MessageBox.Show("Unable to load company information. Invalid company ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Company_Name.Text = loggedUser.CompanyName;
            Company_Email.Text = loggedUser.CompanyEmail;
        }

        private async Task LoadAboutDataAsync()
        {
            About_TextBox.Text = await GetAboutDataAsync();
        }

        private async Task<string> GetAboutDataAsync()
        {
            string content = "";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connection))
                {
                    await connection.OpenAsync();
                    string query = "SELECT content FROM company_info WHERE userId = @userId AND info_type = @infoType";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", MainWindow.CompanyID);
                        cmd.Parameters.AddWithValue("@infoType", "About_Post");

                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                content = reader["content"]?.ToString() ?? "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return content;
        }

        private void SaveData(TextBox textBox, string infoType)
        {
            string queryUpdate = "UPDATE company_info SET content = @content WHERE userId = @userId AND info_type = @infoType";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connection))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(queryUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@content", textBox.Text);
                        command.Parameters.AddWithValue("@infoType", infoType);
                        command.Parameters.AddWithValue("@userId", MainWindow.CompanyID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("No changes were made to the database.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompanyAbout(object sender, RoutedEventArgs e)
        {
            if (abt_you_btn.Content.ToString() == "Edit")
            {
                About_TextBox.IsReadOnly = false;
                About_TextBox.Focus();
                abt_you_btn.Content = "Save";
            }
            else if (abt_you_btn.Content.ToString() == "Save")
            {
                abt_you_btn.Content = "Edit";
                SaveData(About_TextBox, "About_Post");
                About_TextBox.IsReadOnly = true;
            }
        }

        private void BackButton_Btn(object sender, RoutedEventArgs e)
        {
            switch (WindowNumber)
            {
                case 1:
                    new Company_Search().Show();
                    break;
                case 2:
                    new Company_DashBoard().Show();
                    break;
            }
            this.Close();
        }
    }
}
