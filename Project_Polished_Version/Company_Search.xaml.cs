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
    /// Interaction logic for Company_Search.xaml
    /// </summary>
    public partial class Company_Search : Window
    {
        private static string Connection = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%";
        private List<CompanyUser> allCompanies = new List<CompanyUser>();
        public Company_Search()
        {
            InitializeComponent();
            PopulateListBox();
        }
        private void PopulateListBox()
        {
            try
            {
                allCompanies = User_DataBase();
                CompanyListBox.ItemsSource = allCompanies;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error populating the list: " + ex.Message);
            }
        }

        public static List<CompanyUser> User_DataBase()
        {
            List<CompanyUser> companyList = new List<CompanyUser>();


            using (MySqlConnection connection = new MySqlConnection(Connection))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM company_accounts";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CompanyUser item = new CompanyUser
                            {
                                CompanyName = reader["Company_name"].ToString(),
                                CompanyEmail = reader["email"].ToString(),
                                CompanyPassword = reader["password"].ToString(),
                                CompanyAddress = reader["Company_address"].ToString(),
                                CompanyId = Convert.ToInt32(reader["company_id"])
                            };
                            companyList.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message);
                }
            }

            return companyList;
        }

        private void SearchBox_txtchange(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

            string searchText = SearchBox.Text.ToLower();
            var filteredList = allCompanies.Where(c =>
                c.CompanyName.ToLower().Contains(searchText) ||
                c.CompanyEmail.ToLower().Contains(searchText) ||
                c.CompanyAddress.ToLower().Contains(searchText)
            ).ToList();

            CompanyListBox.ItemsSource = filteredList;
        }

        CompanyUser TheSelectedCompany;
        private void Company_Profile(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CompanyListBox.SelectedItem is CompanyUser selectedCompany)
            {
                TheSelectedCompany = selectedCompany;
            }
        }

        private void viewProfile_btn(object sender, RoutedEventArgs e)
        {
            if (CompanyListBox.SelectedItem is CompanyUser TheSelectedCompany)
            {
                Company_Profile cp = new Company_Profile(TheSelectedCompany);
                this.Hide();
                cp.Show();
            }
        }

        private void Back_Button(object sender, RoutedEventArgs e)
        {
            Applicant_DashBoard cdb = new Applicant_DashBoard();
            this.Hide();
            cdb.Show();
        }
    }
}
