using Project_Polished_Version.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Project_Polished_Version
{
    public partial class Applicant_DashBoard : Window
    {
        string connectionString = "Server=127.0.0.1;Database=project_database;UserID=root;Password=SQLDatabase404;";
        public List<ApplicantUser> list = new List<ApplicantUser>();
        public List<string> names = new List<string>();
        public List<NewsFeed> nFeeds = new List<NewsFeed>();
        private DataTable searchData;
        public static int otherUserKey;
        public static int WindowTracker;
        public static bool lb { get; set; }

        public Applicant_DashBoard()
        {
            InitializeComponent();
            InitializeDashboardAsync();
        }

        private async void InitializeDashboardAsync()
        {
            try
            {
                nFeeds = await GetNewsFeedFromDatabaseAsync();
                names = await Task.Run(() => getNames());
                await LoadComboBoxDataAsync();
                await LoadNewsfeedAsync();

                Post_Resume.PostTracker = 1;
                Applicant_Search.As_WindowTracker = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization error: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }
        private List<string> getNames()
        {
            List<string> names = new List<string>();
            foreach (var i in list)
            {
                string fullName = i.First_Name + " " + i.Last_Name;
                names.Add(fullName);
            }
            return names;
        }

        private async Task<List<NewsFeed>> GetNewsFeedFromDatabaseAsync()
        {
            var newsFeed = new List<NewsFeed>();
            // string connectionString = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%";
            try
            {
                using (var connection = new MySqlConnection(connectionString))
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
        private async Task LoadComboBoxDataAsync()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    MessageBox.Show("Connection opened successfully.");

                    string query = "SELECT first_name, last_name FROM applicant_accounts";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        searchData = new DataTable();
                        await Task.Run(() => adapter.Fill(searchData));
                        MessageBox.Show($"Data loaded successfully: {searchData.Rows.Count} rows.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading combo box data: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButton.OK);
            }
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
        private void Searched_Person(object sender, MouseEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is string selectedName)
            {
                if (MainWindow.userAccounts.TryGetValue(selectedName, out var accountName))
                {
                    Applicant_Profile us = new Applicant_Profile();
                    us.ChangeInfo($"{accountName.First_Name} {accountName.Last_Name}", accountName.JobTitle, accountName.Address);
                    otherUserKey = accountName.Id;

                    this.Hide();
                    us.Show();
                }
                else
                {
                    MessageBox.Show("User not found in the dictionary.", "Error", MessageBoxButton.OK);
                }
            }
            else if (e.OriginalSource is TextBlock textBlock)
            {
                MessageBox.Show($"You selected: {textBlock.Text}", "User Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No valid user selected.", "Error", MessageBoxButton.OK);
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

        private void searchJobs_btn(object sender, RoutedEventArgs e)
        {
            SearchJob_Window sjw = new SearchJob_Window();
            this.Hide();
            sjw.Show();
        }

        private void Log_Out_Button(object sender, RoutedEventArgs e)
        {
            //MainWindow.ClearMemory();
            MainWindow loginWindow = new MainWindow();
            this.Close();
            loginWindow.Show();
        }

        private void Application_Check_Button(object sender, RoutedEventArgs e)
        {
            Applicant_Tracker ap = new Applicant_Tracker();
            this.Hide();
            ap.Show();
        }

        private void Profile_Button(object sender, RoutedEventArgs e)
        {
            Applicant_Profile up = new Applicant_Profile();
            Applicant_Profile.windowNumber = 1;
            this.Hide();
            up.Show();
        }

        private void Notification_Button(object sender, RoutedEventArgs e)
        {
            Inbox inbox = new Inbox();
            inbox.Show();

        }

        private void Company_search(object sender, RoutedEventArgs e)
        {
            Company_Profile.WindowNumber = 1;
            Company_Search cs = new Company_Search();
            this.Hide();
            cs.Show();
        }

        private void applicantsearch_btn(object sender, RoutedEventArgs e)
        {
            Applicant_Search applicant_Search = new Applicant_Search();
            Applicant_Profile.windowNumber = 2;
            this.Hide();
            applicant_Search.Show();

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

        private async void Add_Post(object sender, RoutedEventArgs e)
        {
            Post_Resume pr = new Post_Resume();
            pr.Closed += async (s, args) =>
            {
                this.IsEnabled = true;
                await ReloadNewsfeedAsync();
            };
            pr.Show();
        }

        private void Pending_Button(object sender, RoutedEventArgs e)
        {
            Applicant_Tracker ap = new Applicant_Tracker();
            this.Hide();
            ap.Show();
        }

        private void Messages_RB_Checked(object sender, RoutedEventArgs e)
        {
            Messages ap = new Messages();
            this.Hide();
            ap.Show();
        }

        private void Newsfeed_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}



