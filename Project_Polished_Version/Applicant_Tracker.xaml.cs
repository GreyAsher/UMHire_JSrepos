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
    /// Interaction logic for Applicant_Tracker.xaml
    /// </summary>
    public partial class Applicant_Tracker : Window
    {
        private static string Connection = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%";
        private List<Resume> list;
        public Applicant_Tracker()
        {
            InitializeComponent();
            Rclass();
        }
        public static string CompanyName { get; set; }
        private List<Resume> GetResumeFromDatabase()
        {
            List<Resume> pendingFeed = new List<Resume>();
            using (MySqlConnection connection = new MySqlConnection(Connection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM resume_db";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Resume item = new Resume
                            {
                                Applicantion_Id = Convert.ToInt32(reader["Profile_Number"]),
                                CompanyID_Number = Convert.ToInt32(reader["CompanyId"]),
                                Status = reader["Status"].ToString(),
                                Resume_Number = Convert.ToInt32(reader["ResumeNumber"]),
                                userProfile = reader["Profile_Name"].ToString(),
                                Resume_Job_Position = reader["Position"].ToString(),
                                Company_Name = reader["Company_Name"].ToString(),
                                Submitted_Date = DateTime.Now,
                            };
                            pendingFeed.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return pendingFeed;
        }


        private void Rclass()
        {
            list = new List<Resume>();

            foreach (var x in GetResumeFromDatabase())
            {
                if (x.Applicantion_Id == MainWindow.UserID)
                {
                    list.Add(x);
                }
            }

            Job_Sent_Table.ItemsSource = list;

            if (list.Count == 0)
            {
                MessageBox.Show("No data found for the current user.");
            }
        }


        private void Back_Btn_Click(object sender, RoutedEventArgs e)
        {
            Applicant_DashBoard db = new Applicant_DashBoard();
            this.Hide();
            db.Show();
        }
    }
}

