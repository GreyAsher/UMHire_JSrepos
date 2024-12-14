using Project_Polished_Version.Classes;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Windows.Controls;

namespace Project_Polished_Version
{
    /// <summary>
    /// Interaction logic for Friend_List_Window.xaml
    /// </summary>
    public partial class Friend_List_Window : Window
    {
        private string _connection = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%";
        public ObservableCollection<ApplicantUser> ApList { get; set; } = new ObservableCollection<ApplicantUser>();
        public List<int> ConnectList { get; set; } = new List<int>();

        public Friend_List_Window()
        {
            InitializeComponent();
            this.DataContext = this; // Set the DataContext for binding
            friendsList.ItemsSource = ApList; // Bind the list to the UI
            LoadDataAsync(); // Initial load
        }

        private async void LoadDataAsync()
        {
            try
            {
                await Task.Run(() => FetchFriendIDs("Pending")); // Load pending requests by default
                BindToUI();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}");
            }
        }

        private void FetchFriendIDs(string statusFilter)
        {
            ConnectList.Clear(); // Clear the list to avoid duplicates
            using (MySqlConnection connection = new MySqlConnection(_connection))
            {
                string query = @"SELECT user_id FROM friends 
                                 WHERE friend_id = @userID AND status = @StatusFilter";
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userID", MainWindow.UserID);
                    command.Parameters.AddWithValue("@StatusFilter", statusFilter);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ConnectList.Add(reader.GetInt32("user_id"));
                        }
                    }
                }
            }
        }

        private void BindToUI()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ApList.Clear(); // Clear the ObservableCollection
                using (MySqlConnection connection = new MySqlConnection(_connection))
                {
                    string query = @"SELECT u.id, u.first_name, u.last_name 
                                     FROM applicant_accounts u
                                     WHERE u.id = @FriendID";
                    connection.Open();
                    foreach (var friendID in ConnectList)
                    {
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FriendID", friendID);
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var user = new ApplicantUser
                                    {
                                        Id = reader.GetInt32("id"),
                                        First_Name = reader.GetString("first_name"),
                                    };
                                    ApList.Add(user); // Add user to the ObservableCollection
                                }
                            }
                        }
                    }
                }
            });
        }

        private void Friend_Request_Toggle(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Connected_Applicant_Button.Content.ToString() == "Connect Requests")
                {
                    Connected_Applicant_Button.Content = "Connect Accepts";
                    FetchFriendIDs("Pending"); // Fetch pending friend requests
                }
                else if (Connected_Applicant_Button.Content.ToString() == "Connect Accepts")
                {
                    Connected_Applicant_Button.Content = "Connect Requests";
                    FetchFriendIDs("Accepted"); // Fetch accepted friends
                }
                BindToUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling friend requests: {ex.Message}");
            }
        }

        private void Accept_Friend(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ApplicantUser user) // Ensure proper context
            {
                using (MySqlConnection connection = new MySqlConnection(_connection))
                {
                    string query = @"UPDATE friends
                                     SET status = 'Accepted'
                                     WHERE (user_id = @CurrentUserId AND friend_id = @FriendId)
                                        OR (user_id = @FriendId AND friend_id = @CurrentUserId)";

                    try
                    {
                        connection.Open();
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@CurrentUserId", MainWindow.UserID);
                            command.Parameters.AddWithValue("@FriendId", user.Id);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Friend request from {user.First_Name} accepted.");
                                ApList.Remove(user); // Remove from the ObservableCollection
                            }
                            else
                            {
                                MessageBox.Show("Failed to accept the friend request. Please try again.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid action or data. Please try again.");
            }
        }
    }
}
