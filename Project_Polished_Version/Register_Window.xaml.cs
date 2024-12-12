using MySql.Data.MySqlClient;
using Project_Polished_Version.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Project_Polished_Version
{
    /// <summary>
    /// Interaction logic for Register_Window.xaml
    /// </summary>
    public partial class Register_Window : Window
    {
        public Register_Window()
        {
            InitializeComponent();
        }

        private void AddAccount()
        {
            try
            {      
                string First_Name = First_Name_txtbox.Text;
                string Last_Name = Last_Name_txtBox.Text;
                string Email = emal_txtBox.Text;
                string JobTitle = Job_Position_txtBox.Text;
                string Password = PassWord_txtBox.Password;
                string Gender = Male_Option.IsChecked == true ? "Male" : "Female";
                string Mobile_Number = Mobile_Number_txtBox.Text;
                string address = Address_TxtBox.Text;
                int id = 1001;
                //Convert.ToInt32(Mobile_Number_txtBox.Text);
                if (string.IsNullOrWhiteSpace(First_Name) || string.IsNullOrWhiteSpace(Last_Name) ||
                   string.IsNullOrWhiteSpace(JobTitle) || string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Mobile_Number) || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("All fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Mobile_Number.Length == 11 || Mobile_Number[0].Equals(0) || Mobile_Number[1].Equals(9))
                {
                    MessageBox.Show("Invalid Phone number, Phone Number should start at 09");
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;" +
            "Database=project_database;UserName= root;" +
            "Password=SQLDatabase404"))
                {
                    string query = "INSERT INTO applicant_accounts (first_name, last_name, email, gender, Phone_Number,Job_Title,`password`" +
                   ",address) " +
                              "VALUES (@first_name, @last_name, @email, @gender, @Phone_Number,@Job_Title,@password,@address)";
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        //cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@first_name", First_Name);
                        cmd.Parameters.AddWithValue("@last_name", Last_Name);
                        cmd.Parameters.AddWithValue("@gender", Gender);
                        cmd.Parameters.AddWithValue("@Job_Title", JobTitle);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.Parameters.AddWithValue("@Phone_Number", Mobile_Number);
                        cmd.Parameters.AddWithValue("@password", Password);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Next_btn(object sender, RoutedEventArgs e)
        {
            AddAccount();
        }
    }
}



