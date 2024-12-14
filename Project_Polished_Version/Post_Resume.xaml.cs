using MySql.Data.MySqlClient;
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
    /// Interaction logic for Post_Resume.xaml
    /// </summary>
    public partial class Post_Resume : Window
    {
        public Post_Resume()
        {
            InitializeComponent();
        }
        public static int PostTracker { get; set; }
        private string name;

        private void InsertPost()
        {
            if (string.IsNullOrWhiteSpace(PostTextBox.Text))
            {
                MessageBox.Show("Post content cannot be empty.");
                return;
            }

            try
            {
                string selectQuery = "SELECT MAX(post_id) AS max_id FROM posts";
                string insertQuery = "INSERT INTO posts (content, user_id, post_id, created_at, company_name) " +
                                     "VALUES (@content, @user_id, @post_id, @created_at, @company_name)";
                switch (PostTracker)
                {
                    case 1:
                        name = MainWindow.userAccountsGetID[MainWindow.UserID].First_Name + " " +
                               MainWindow.userAccountsGetID[MainWindow.UserID].Last_Name;
                        break;
                    case 2:
                        name = MainWindow.companyAccountsGetID[MainWindow.UserID].CompanyName;
                        break;
                }

                int postedId = 0;

                using (MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;Database=project_database;UserName=root;Password=SQLDatabase404"))
                {
                    connection.Open();

                    // Fetch the next post_id
                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                postedId = reader["max_id"] != DBNull.Value ? Convert.ToInt32(reader["max_id"]) + 1 : 1;
                            }
                        }
                    }

                    // Insert the new post
                    using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@content", PostTextBox.Text);
                        insertCommand.Parameters.AddWithValue("@user_id", MainWindow.UserID);
                        insertCommand.Parameters.AddWithValue("@post_id", postedId);
                        insertCommand.Parameters.AddWithValue("@created_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCommand.Parameters.AddWithValue("@company_name", name);

                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Post added successfully.");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add post.");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void Post_btn_click(object sender, RoutedEventArgs e)
        {
            InsertPost();
        }

        private void Cancel_Post_btn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}


