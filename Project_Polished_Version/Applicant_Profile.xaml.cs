using Microsoft.Win32;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Project_Polished_Version.Classes
{
    public partial class Applicant_Profile : Window
    {
        private string _ConnectionData = "Server=127.0.0.1;Database=project_database;UserID=root;Password=SQLDatabase404;";
        public static int windowNumber { get; set; }
        public static int searchUserKey { get; set; }
        public static List<int> keys = new List<int>();
        public ApplicantUser CurrentResume { get; private set; }
        public Applicant_Profile()
        {
            InitializeComponent();
            ShowInfo();
            LoadAboutSection();
            LoadExperienceSection();
            LoadEducationSection();
            LoadProfilePicture("/Images/Lorenz.jpg");
            About_TextBox.Text = "Enter text here";
        }


        private async Task CheckFriend()
        {

        }

        private void PopulateProfile(ApplicantUser resume)
        {
            disableButton();
            try
            {
                if (resume == null)
                {
                    throw new ArgumentNullException(nameof(resume), "The resume object is null.");
                }

                // Load profile picture
                LoadProfilePicture(resume.Applicant_Photo);

                // Populate other profile data
                Full_Name.Text = $"{resume.First_Name} {resume.Last_Name}";
                Job_Title_txtbox.Text = resume.JobTitle;
                Address_txtbox.Text = resume.Address;
                searchUserKey = resume.Id;
                // Add any additional properties
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating profile: {ex.Message}");
            }
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
        private void AddFriend_Btn(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(_ConnectionData))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO friends (user_id, friend_id) VALUES (@UserId, @FriendId)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", MainWindow.UserID);
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
            Connect_Friend_Btn.Content = "Pending";

        }

        private void showFriendList_btn(object sender, RoutedEventArgs e)
        {
            Friend_List_Window flw = new Friend_List_Window();
            flw.Show();
        }

        public Applicant_Profile(ApplicantUser userInfo)
        {
            CurrentResume = userInfo;
            PopulateProfile(userInfo);
            InitializeComponent();
            LoadProfilePicture("/Images/Lorenz.jpg");
            Full_Name.Text = $"{userInfo.First_Name} {userInfo.Last_Name}";
            Job_Title_txtbox.Text = userInfo.JobTitle;
            Address_txtbox.Text = userInfo.Address;
            searchUserKey = userInfo.Id;
            disableButton();

            if (userInfo == null)
            {
                MessageBox.Show("User information cannot be null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                PopulateProfile(userInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing profile: " + ex.Message);
            }
        }

        private void ShowInfo()
        {
            int key = MainWindow.UserID;

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

        private async void LoadAboutSection()
        {
            About_TextBox.Text = await getAboutDataAsync("About_Post");
        }

        private async void LoadEducationSection()
        {
            Education_TextBox_Copy.Text = await getAboutDataAsync("Education_Post");
        }

        private async void LoadExperienceSection()
        {
            Experience_TextBox.Text = await getAboutDataAsync("Experience_Post");
        }

        private async Task<string> getAboutDataAsync(string infoType)
        {
            string content = "";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_ConnectionData))
                {
                    await connection.OpenAsync();

                    string query = "SELECT content FROM applicant_info WHERE userId = @userId AND info_type = @infoType";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", MainWindow.UserID);
                        cmd.Parameters.AddWithValue("@infoType", infoType);

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
        private void sameThing(string infoType, TextBox tbx, Button btn)
        {
            // Check the current state of the button
            if (btn.Content.ToString() == "Edit")
            {
                tbx.IsReadOnly = false;
                tbx.Focus();
                btn.Content = "Save";
                MessageBox.Show("Show: " + MainWindow.UserID);
            }
            else if (btn.Content.ToString() == "Save")
            {
                btn.Content = "Edit";
                SaveData(tbx, infoType);
                tbx.IsReadOnly = true;

            }
        }
        private void SaveAbout_Click(object sender, RoutedEventArgs e)
        {
            sameThing("About_Post", About_TextBox, abt_you_btn);
        }

        private void SaveEducation_Click(object sender, RoutedEventArgs e)
        {
            sameThing("Education_Post", Education_TextBox_Copy, SaveEducation_btn);
        }

        private void SaveExperience_Click(object sender, RoutedEventArgs e)
        {
            sameThing("Experience_Post", Experience_TextBox, Edit_Experience_btn);
        }

        private void SaveData(TextBox textBox, string infoType)
        {
            string queryUpdate = "UPDATE applicant_info SET content = @content WHERE userId = @userId AND info_type = @infoType";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_ConnectionData))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(queryUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@content", textBox.Text);
                        command.Parameters.AddWithValue("@infoType", infoType);
                        command.Parameters.AddWithValue("@userId", MainWindow.UserID);

                        // Execute the query
                        int rowsAffected = command.ExecuteNonQuery();

                        // Optionally notify the user
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



        private void disableButton()
        {
            using (MySqlConnection connection = new MySqlConnection(_ConnectionData))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM friends WHERE (user_id = @UserId AND friend_id = @FriendId) " +
                                   "OR (user_id = @FriendId AND friend_id = @UserId)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", MainWindow.UserID);
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

        private void Edit_Profile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a Profile Picture",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                try
                {
                    // Display the selected image in the Image control
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(selectedFilePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    User_Profile.Source = bitmap;

                    // Save the image to the database
                    SaveImage_IntoDatabase(MainWindow.UserID, selectedFilePath);
                    MessageBox.Show("Profile picture saved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error processing the image: " + ex.Message);
                }
            }
        }

        private BitmapImage LoadImageFromDatabase(int userId)
        {
            byte[] imageBytes = null;

            using (MySqlConnection connection = new MySqlConnection(_ConnectionData))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT profile_picture FROM applicant_accounts WHERE id = @UserId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            imageBytes = (byte[])reader["Profile_Picture"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }

            if (imageBytes != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                return bitmap;
            }

            return null;
        }

        private void SaveImage_IntoDatabase(int userId, string filePath)
        {
            byte[] imageBytes = File.ReadAllBytes(filePath);

            using (MySqlConnection connection = new MySqlConnection(_ConnectionData))
            {
                try
                {
                    connection.Open();

                    string query = "UPDATE applicant_accounts SET profile_picture = @ProfilePicture WHERE id = @UserId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@ProfilePicture", imageBytes);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Profile picture saved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving image: " + ex.Message);
                }
            }
        }

        private BitmapImage GetImageFromDatabase(int userId)
        {
            byte[] imageBytes = null;

            using (MySqlConnection connection = new MySqlConnection(_ConnectionData))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT profile_picture FROM applicant_accounts WHERE id = @UserId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            imageBytes = (byte[])reader["profile_picture"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching profile picture: " + ex.Message);
                }
            }

            if (imageBytes != null)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }
                    return bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error converting image: " + ex.Message);
                }
            }

            // Return default image if no data
            return new BitmapImage(new Uri("/Images/default-profile.png", UriKind.Relative));
        }

        private void LoadProfilePicture(string photoPath)
        {
            try
            {
                // Check if the photo path is null, empty, or if the file does not exist
                if (string.IsNullOrEmpty(photoPath) || !File.Exists(photoPath))
                {
                    // Set a default image
                    User_Profile.Source = new BitmapImage(new Uri("/Images/default-profile.png", UriKind.Relative));
                }
                else
                {
                    // Load the specified profile picture
                    User_Profile.Source = new BitmapImage(new Uri(photoPath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile picture: {ex.Message}");
                // Fallback to the default profile picture
                User_Profile.Source = new BitmapImage(new Uri("/Images/default-profile.png", UriKind.Relative));
            }
        }
    }
}
