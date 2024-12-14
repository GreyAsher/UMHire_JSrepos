using MySql.Data.MySqlClient;
using Project_Polished_Version.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Project_Polished_Version
{
    /// <summary>
    /// Interaction logic for Company_DashBoard.xaml
    /// </summary>
    public partial class Company_DashBoard : Window
    {
        private string _connection = "Server=localhost;" +
            "Database=project_database;UserName= root;" +
            "Password=Cedric1234%%";

        public List<NewsFeed> nFeeds = new List<NewsFeed>();
        public Company_DashBoard()
        {
            InitializeComponent();
            LoadNewsfeedAsync();
            Applicant_Search.As_WindowTracker = 2;
            Company_Profile.WindowNumber = 2;
        }

        private async Task ReloadNewsfeedAsync()
        {
            try
            {
                Newsfeed_ListBox.Items.Clear();
                nFeeds = await GetNewsFeedFromDatabaseAsync();

                foreach (var newsFeed in nFeeds)
                {
                    Newsfeed_ListBox.Items.Add(CreateNewsfeedItem(newsFeed.Author, newsFeed.Content));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reloading newsfeed: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        private ListBoxItem CreateNewsfeedItem(string userName, string content)
        {
            var border = new Border
            {
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/ARTBLUE.jpg")),
                    Stretch = Stretch.UniformToFill
                },
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                Child = new Grid
                {
                    RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // userName and content
                new RowDefinition { Height = GridLength.Auto }  // buttons
            },
                    Children =
            {
                // UserName and Content Section
                new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = userName,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(5, 0, 5, 5),
                            Foreground = Brushes.White
                        },
                        new TextBlock
                        {
                            Text = content,
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(5, 0, 5, 0),
                            Foreground = Brushes.White
                        }
                    }
                }

                    }
                },

                // Buttons Section
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 5, 0, 0),
                    Children =
                    {
                        new Button
                        {
                            Content = "View Comments",
                            Margin = new Thickness(5, 0, 5, 0),
                            Padding = new Thickness(10, 5, 10, 5)
                        },
                        new Button
                        {
                            Content = "View Details",
                            Margin = new Thickness(5, 0, 0, 0),
                            Padding = new Thickness(10, 5, 10, 5)
                        }
                    }
                }
            }
                }
            };

            // Set rows for the grid children
            Grid.SetRow(border.Child, 0); // UserName and Content
            Grid.SetRow((border.Child as Grid)?.Children[1], 1); // Buttons Section

            return new ListBoxItem { Content = border, Height = 100 };
        }

        private async Task<List<NewsFeed>> GetNewsFeedFromDatabaseAsync()
        {
            var newsFeed = new List<NewsFeed>();
            // string connectionString = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%";
            try
            {
                using (var connection = new MySqlConnection(_connection))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM posts";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            newsFeed.Add(new NewsFeed
                            {
                                Author = reader["company_name"].ToString(),
                                Content = reader["Content"].ToString(),
                                Photos = "Enter Photo here",
                                Id = Convert.ToInt32(reader["post_id"]),
                                UserID = Convert.ToInt32(reader["user_id"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching news feed: {ex.Message}", "Error", MessageBoxButton.OK);
            }

            return newsFeed;
        }

        private async Task LoadNewsfeedAsync()
        {
            try
            {
                nFeeds = await GetNewsFeedFromDatabaseAsync();
                Newsfeed_ListBox.Items.Clear();

                foreach (var newsFeed in nFeeds)
                {
                    Newsfeed_ListBox.Items.Add(CreateNewsfeedItem(newsFeed.Author, newsFeed.Content));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading newsfeed: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Click_Profile(object sender, RoutedEventArgs e)
        {
            Company_Profile cp = new Company_Profile();
            this.Hide();
            cp.Show();
        }

        private void applicantsearch_btn(object sender, RoutedEventArgs e)
        {
            Applicant_Search ap = new Applicant_Search();
            this.Hide();
            ap.Show();
        }

        private void application_btn(object sender, RoutedEventArgs e)
        {

        }

        private void Log_out_btn(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void Messages_RB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Applicant_Check_btn(object sender, RoutedEventArgs e)
        {
            Company_Check cc = new Company_Check();
            this.Close();
            cc.Show();
        }
    }
}
