using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project_Polished_Version.Classes
{
    public partial class Applicant_Profile : Window
    {
        public static int windowNumber { get; set; }
        public static int searchUserKey { get; set; }
        public static List<int> keys = new List<int>();

        public Applicant_Profile()
        {
            InitializeComponent();
            ShowInfo();
            LoadAboutSection(); // Avoid using 'await' in constructors
        }

        public Applicant_Profile(ApplicantUser userInfo)
        {
            InitializeComponent();

            // Set user information on the UI
            Full_Name.Text = $"{userInfo.First_Name} {userInfo.Last_Name}";
            Job_Title_txtbox.Text = userInfo.JobTitle;
            Address_txtbox.Text = userInfo.Address;
            searchUserKey = userInfo.Id;
            disableButton();
        }

        private void disableButton()
        {
            using (MySqlConnection connection = new MySqlConnection(
                "Server=127.0.0.1;Database=project_database;User=root;Password=SQLDatabase404"))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM friends WHERE (user_id = @UserId AND friend_id = @FriendId) " +
                                   "OR (user_id = @FriendId AND friend_id = @UserId)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", MainWindow.userID);
                        cmd.Parameters.AddWithValue("@FriendId", searchUserKey);

                        int friendshipCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (friendshipCount > 0)
                        {
                            Connect_Friend_Btn.IsEnabled = false;
                            Connect_Friend_Btn.Content = "Already Connected";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while checking the friendship: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowInfo()
        {
            int key = MainWindow.userID;

            if (MainWindow.userAccountsGetID.TryGetValue(key, out ApplicantUser logedUser))
            {
                Full_Name.Text = $"{logedUser.First_Name} {logedUser.Last_Name}";
                Job_Title_txtbox.Text = logedUser.JobTitle;
                Address_txtbox.Text = logedUser.Address;
            }
        }

        public void ChangeInfo(string name, string jobTitle, string address)
        {
            Full_Name.Text = name;
            Job_Title_txtbox.Text = jobTitle;
            Address_txtbox.Text = address;
        }
        int ClickNumber = 0;
        private void abt_you_btn_Click(object sender, RoutedEventArgs e)
        {
            if (ClickNumber % 2 == 0)
            {
                abt_you_btn.Content = "Save";
                About_TextBox.IsReadOnly = false;
                About_TextBox.Focus();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(About_TextBox.Text))
                {
                    MessageBox.Show("About section cannot be empty. Please provide valid content.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                abt_you_btn.Content = "Edit";
                About_TextBox.IsReadOnly = true;
                insertData();
            }

            ClickNumber++;
        }

        private void AddFriend_Btn(object sender, RoutedEventArgs e)
        {
            string connectionString = "Server=127.0.0.1;Database=project_database;User=root;Password=SQLDatabase404";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO friends (user_id, friend_id) VALUES (@UserId, @FriendId)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", MainWindow.userID);
                        cmd.Parameters.AddWithValue("@FriendId", searchUserKey);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        MessageBox.Show(rowsAffected > 0 ? "Friend request sent successfully!" : "Failed to send friend request.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void showFriendList_btn(object sender, RoutedEventArgs e)
        {
            Friend_List_Window flw = new Friend_List_Window();
            flw.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window targetWindow;

                if (windowNumber == 1)
                {
                    targetWindow = new Applicant_DashBoard();
                }
                else if (windowNumber == 2)
                {
                    targetWindow = new Applicant_Search();
                }
                else
                {
                    throw new InvalidOperationException("Invalid window number.");
                }

                this.Hide();
                targetWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while navigating back: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        private void CreateEducationTXT_box()
        {
            TextBox Add_Education_ntb = new TextBox
            {
                Text = "",
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Height = 100,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // Example: Uncomment and add this to your StackPanel or another container
            // Education_StackPanel.Children.Add(Add_Education_ntb);
        }

        private void Add_new_Education(object sender, RoutedEventArgs e)
        {
            CreateEducationTXT_box();
        }

        private void insertData()
        {
            // Query for inserting data
            string queryInsert = "INSERT INTO applicant_info (userid, info_type, content,date_started) VALUES (@userId, @infoType, @content,@Dstart)";

            // Database connection string
            string connectionString = "Server=localhost;Database=project_database;User=root;Password=Cedric1234%%";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    if (string.IsNullOrWhiteSpace(About_TextBox.Text))
                    {
                        MessageBox.Show("The about section cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Insert the record
                    using (MySqlCommand cmdInsert = new MySqlCommand(queryInsert, connection))
                    {
                        cmdInsert.Parameters.AddWithValue("@userId", MainWindow.userID);
                        cmdInsert.Parameters.AddWithValue("@infoType", "About_Post");
                        cmdInsert.Parameters.AddWithValue("@content", About_TextBox.Text);
                        cmdInsert.Parameters.AddWithValue("@Dstart", DateTime.Now.ToString());

                        int rowsAffected = cmdInsert.ExecuteNonQuery();

                        MessageBox.Show(rowsAffected > 0
                            ? "About section added successfully!"
                            : "Failed to add about section. Please try again.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while inserting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void LoadAboutSection()
        {
            string aboutContent = await getAboutDataASYNC();
            About_TextBox.Text = aboutContent;
        }


        private async Task<string> getAboutDataASYNC()
        {
            string content = "";
            string connectionString = "Server=127.0.0.1;Database=project_database;User=root;Password=SQLDatabase404";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string query = "SELECT content FROM applicant_info WHERE userId = @userId AND info_type = @infotype;";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", MainWindow.userID);
                        cmd.Parameters.AddWithValue("@infotype", "About_Post");

                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                content = reader.GetString("content");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error retrieving data: {ex.Message}");
                }
            }

            return content;
        }

        private void Education_Edit_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Edit_Experience_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
