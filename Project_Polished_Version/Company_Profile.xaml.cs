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
    /// <summary>
    /// Interaction logic for Company_Profile.xaml
    /// </summary>
    public partial class Company_Profile : Window
    {
        public Company_Profile(CompanyUser company)
        {
            InitializeComponent();
            LoadJobfeedAsync();
            Company_Name.Text = company.CompanyName;
            Company_Email.Text = company.CompanyEmail;
            //Company_Address.Text = company.CompanyAddress;
        }
        public static int WindowNumber { get; set; }
        public Company_Profile()
        {
            InitializeComponent();
            LoadJobfeedAsync();
            ShowInfo();
        }

        private void ShowInfo()
        {
            int key = MainWindow.CompanyID;
            if (MainWindow.companyAccountsGetID.TryGetValue(key, out CompanyUser loggedUser))
            {
                Company_Name.Text = loggedUser.CompanyName;
                Company_Email.Text = loggedUser.CompanyEmail;
               // Company_Address.Text = loggedUser.CompanyAddress;
            }
        }

        private ListBoxItem CreateJobPost(string userName, string content)
        {
            var listBoxItem = new ListBoxItem();
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(213, 160, 33)),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Height = 100,
                Margin = new Thickness(5)
            };

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            var userNameTextBlock = new TextBlock
            {
                Text = userName,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 0, 5, 5)
            };

            var contentTextBlock = new TextBlock
            {
                Text = content,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 0, 5, 0)
            };
            stackPanel.Children.Add(userNameTextBlock);
            stackPanel.Children.Add(contentTextBlock);
            border.Child = stackPanel;
            listBoxItem.Content = border;

            return listBoxItem;
        }

        private async void LoadJobfeedAsync()
        {
            try
            {
                var jobItems = await Task.Run(() => Company_DashBoard.Job_DataBase());
                foreach (var i in jobItems)
                {
                    if (i.Company_userNumber == MainWindow.userID)
                    {
                        //Newsfeed_ListBox.Items.Add(CreateJobPost(i.Job_Position, i.Job_Description));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading job feed: {ex.Message}");
            }
        }

        private void BackButton(object sender, RoutedEventArgs e)
        {

        }

        private void Profile_RB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void BackButton_btn(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Show: " + WindowNumber);
            switch (WindowNumber)
            {
                case 1:
                    Company_Search cs = new Company_Search();
                    this.Hide();
                    cs.Show();
                    break;

                case 2:
                    Company_DashBoard sjw = new Company_DashBoard();
                    this.Hide();
                    sjw.Show();
                    break;
            }
        }

        private void Log_Out_RB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Pending_Applications_RB_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void abt_you_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
