using MySql.Data.MySqlClient;
using Project_Polished_Version.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_Polished_Version
{
    /// <summary>
    /// Interaction logic for Applicant_Search.xaml
    /// </summary>
    public partial class Applicant_Search : Window
    {
        private List<ApplicantUser> allUsers = new List<ApplicantUser>();
        public static int WindowTracker;
        public Applicant_Search()
        {
            InitializeComponent();
            PopulateListBox();
        }
        private void PopulateListBox()
        {
            allUsers = User_DataBase();
            JobList.ItemsSource = allUsers;
        }
        public static List<ApplicantUser> User_DataBase()
        {
            List<ApplicantUser> applicantFeed = new List<ApplicantUser>();
            using (MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;" + "Database=project_database;UserName= root;" +
                        "Password=SQLDatabase404"))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM applicant_accounts";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

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
                                Id = Convert.ToInt32(reader["id"])
                            };
                            applicantFeed.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
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

            if (JobList.SelectedItem is ApplicantUser selectedUser)
            {
                this.Hide();
                Applicant_Profile.windowNumber = 2;

                Applicant_Profile userSelected = new Applicant_Profile(selectedUser);
                userSelected.Show();
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

