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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Project_Polished_Version
{

    public partial class SearchJob_Window : Window
    {
        private static string connectionString = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%;Pooling=true";
        public SearchJob_Window()
        {
            InitializeComponent();
            PopulateListBox();
        }
        private List<Jobs> allJobs = new List<Jobs>();
        private async void PopulateListBox()
        {
            allJobs = await Job_DataBaseAsync();
            JobList.ItemsSource = allJobs;
        }

        public static async Task<List<Jobs>> Job_DataBaseAsync()
        {
            List<Jobs> jobFeed = new List<Jobs>();



            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string query = "SELECT Job_Position, Job_Description, is_filled, Job_id, Company_userNumber FROM jobs_db LIMIT 1000";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            jobFeed.Add(new Jobs
                            {
                                Job_Position = reader["Job_Position"].ToString(),
                                Job_Description = reader["Job_Description"].ToString(),
                                IsFilled = reader["is_filled"].ToString(),
                                Job_id = Convert.ToInt32(reader["Job_id"]),
                                Company_userNumber = Convert.ToInt32(reader["Company_userNumber"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

            return jobFeed;
        }

        Jobs TheSelectedJobs;

        private void SearchBox_txtchange(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

            string searchText = SearchBox.Text.ToLower();
            var filteredList = allJobs.Where(c =>
                c.Job_Position.ToLower().Contains(searchText) ||
                c.Job_Description.ToLower().Contains(searchText)
            ).ToList();

            JobList.ItemsSource = filteredList;
        }

        private int Company_Number = 0;
        int _Resume_Number = 1;
        private void getJobs()
        {
            try
            {
                string query = "INSERT INTO resume_db (Profile_Name, Profile_Number, Entry_Time, Status, CompanyId, ResumeNumber,Email,Position,Company_Name) " +
                               "VALUES (@Profile_Name, @Profile_Number, @Entry_Time, @Status, @CompanyId, @ResumeNumber,@Email,@Position,@CompanyName)";

                string name = MainWindow.userAccountsGetID[MainWindow.UserID].First_Name + " " +
                    MainWindow.userAccountsGetID[MainWindow.UserID].Last_Name;

                Resume Resume_Class = new Resume()
                {
                    userProfile = name,
                    CompanyID_Number = Company_Number,
                    Applicant_Number = MainWindow.UserID,
                    Submitted_Date = DateTime.Now,
                    Status = "Pending",
                    Resume_Number = _Resume_Number,
                    Resume_Job_Position = MainWindow.userAccountsGetID[MainWindow.UserID].JobTitle,
                    Company_Name = MainWindow.companyAccountsGetID[Company_Number].CompanyName,
                    Email = MainWindow.UserEmail
                };

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@Profile_Name", Resume_Class.userProfile);
                        command.Parameters.AddWithValue("@Profile_Number", Resume_Class.Applicant_Number);
                        command.Parameters.AddWithValue("@Entry_Time", Resume_Class.Submitted_Date);
                        command.Parameters.AddWithValue("@Status", Resume_Class.Status);
                        command.Parameters.AddWithValue("@Position", Resume_Class.Resume_Job_Position);
                        command.Parameters.AddWithValue("@CompanyName", Resume_Class.Company_Name);
                        command.Parameters.AddWithValue("@CompanyId", Resume_Class.CompanyID_Number);
                        command.Parameters.AddWithValue("@ResumeNumber", Resume_Class.Resume_Number);
                        command.Parameters.AddWithValue("@Email", Resume_Class.Email);
                        // Execute the query
                        int rowsAffected = command.ExecuteNonQuery();

                        // Check if data was inserted
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data inserted successfully!");
                        }
                        else
                        {
                            MessageBox.Show("No data was inserted.");
                        }
                    }
                }
            }
            catch (Exception e) { MessageBox.Show("Error: " + e.Message); }
        }

        private void Search_btn(object sender, RoutedEventArgs e)
        {
            if (JobList.SelectedItem is Jobs selectedCompany)
            {
                getJobs();
            }
        }

        private void Search_selectionChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (JobList.SelectedItem is Jobs selectedJobs)
            {
                Company_Number = selectedJobs.Company_userNumber;
                TheSelectedJobs = selectedJobs;
            }
        }

        private void Back_Button(object sender, RoutedEventArgs e)
        {
            Applicant_DashBoard db = new Applicant_DashBoard();
            this.Hide();
            db.Show();
        }
    }
}

