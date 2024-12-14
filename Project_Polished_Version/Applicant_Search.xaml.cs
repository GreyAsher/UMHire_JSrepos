using MySql.Data.MySqlClient;
using Project_Polished_Version.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Project_Polished_Version
{
    /// <summary>
    /// Interaction logic for Applicant_Search.xaml
    /// </summary>
    public partial class Applicant_Search : Window
    {
        private static string Connection = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%";
        private List<ApplicantUser> allUsers = new List<ApplicantUser>();
        public static int WindowTracker;

        public Applicant_Search()
        {
            InitializeComponent();
            PopulateListBox();
        }

        private void PopulateListBox()
        {
            int loggedInUserId = MainWindow.UserID; // Get the logged-in user's ID
            allUsers = User_DataBase(loggedInUserId); // Fetch users except the logged-in user
            JobList.ItemsSource = allUsers;
        }

        public static List<ApplicantUser> User_DataBase(int excludeUserId)
        {
            List<ApplicantUser> applicantFeed = new List<ApplicantUser>();

            using (MySqlConnection connection = new MySqlConnection(Connection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM applicant_accounts WHERE id != @ExcludeUserId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ExcludeUserId", excludeUserId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ApplicantUser item = new ApplicantUser
                            {
                                First_Name = reader["first_name"].ToString(),
                                Last_Name = reader["last_name"].ToString(),
                                Email = reader["email"].ToString(),
                                JobTitle = reader["Job_Title"].ToString(),
                                Password = reader["password"].ToString(),
                                PhoneNumber = reader["Phone_Number"].ToString(),
                                Gender = reader["gender"].ToString(),
                                Address = reader["address"].ToString(),
                                Id = Convert.ToInt32(reader["id"]),
                                Applicant_Photo = reader["Profile_Picture"] != DBNull.Value ? reader["Profile_Picture"].ToString() : null
                            };
                            applicantFeed.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving data from the database: " + ex.Message);
                }
            }

            return applicantFeed;
        }


        ApplicantUser user;
        private void Company_Profile(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (JobList.SelectedItem is ApplicantUser selectedUser)
            {
                user = selectedUser;
                Applicant_Profile userSelected = new Applicant_Profile(selectedUser);
                userSelected.Show();
            }
        }

        private void SearchBox_txtchange(object sender, RoutedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            var filteredList = allUsers.Where(c =>
                c.First_Name.ToLower().Contains(searchText) ||
                c.Last_Name.ToLower().Contains(searchText)
            ).ToList();

            JobList.ItemsSource = filteredList;
        }

        private void viewProfile_btn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (JobList.SelectedItem is ApplicantUser selectedUser)
                {
                    if (selectedUser == null)
                    {
                        throw new NullReferenceException("The selected user is null.");
                    }

                    // Open the Applicant_Profile window
                    this.Hide();
                    Applicant_Profile userProfile = new Applicant_Profile(selectedUser);
                    userProfile.Show();
                }
                else
                {
                    MessageBox.Show("Please select a valid user from the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}\nStackTrace: {ex.StackTrace}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static int As_WindowTracker;

        private void Back_Button(object sender, RoutedEventArgs e)
        {
            switch (As_WindowTracker)
            {
                case 1:
                    Applicant_DashBoard db = new Applicant_DashBoard();
                    this.Hide();
                    db.Show();
                    break;
                case 2:
                    Company_DashBoard cd = new Company_DashBoard();
                    this.Hide();
                    cd.Show();
                    break;
            }
        }
    }
}
