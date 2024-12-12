using Project_Polished_Version.Classes;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System;

namespace Project_Polished_Version
{
    /// <summary>
    /// Interaction logic for Friend_List_Window.xaml
    /// </summary>
    public partial class Friend_List_Window : Window
    {
        public ObservableCollection<ApplicantUser> ApList { get; set; } = new ObservableCollection<ApplicantUser>();
        public List<int> ConnectList { get; set; } = new List<int>();

        public Friend_List_Window()
        {
            InitializeComponent();
            friendsList.ItemsSource = ApList; // Bind UI to ObservableCollection
            LoadDataAsync(); // Load data asynchronously
        }

        private async void LoadDataAsync()
        {
            try
            {
                // Fetch friends and bind them to UI
                await Task.Run(() => FetchFriendIDs());
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

        private void FetchFriendIDs()
        {
            // Fetch friend IDs from the database
            using (MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;Database=project_database;User=root;Password=SQLDatabase404"))
            {
                string query = "SELECT friend_id FROM friends WHERE user_id = @userID";
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userID", MainWindow.userID);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ConnectList.Add(reader.GetInt32("friend_id"));
                        }
                    }
                }
            }
        }

        private void BindToUI()
        {
            // Fetch ApplicantUser data from the dictionary and add to ObservableCollection
            foreach (var friendID in ConnectList)
            {
                if (MainWindow.userAccountsGetID.TryGetValue(friendID, out ApplicantUser user))
                {
                    Application.Current.Dispatcher.Invoke(() => ApList.Add(user));
                }
            }
        }
    }
}
