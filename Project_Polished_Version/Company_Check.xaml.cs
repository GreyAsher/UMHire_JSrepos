using MySql.Data.MySqlClient;
using Project_Polished_Version.Classes;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.IO;
using DSA1.Main;

/// <summary>
/// Interaction logic for Company_Check.xaml
/// </summary>
namespace Project_Polished_Version
    {
        public partial class Company_Check : Window
        {
            private List<Resume> list;
            private string _connection = "Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%";
            public ICommand ViewProfileCommand { get; }

            public Company_Check()
            {
                InitializeComponent();
                ViewProfileCommand = new RelayCommand<ApplicantUser>(ViewProfile);
                DataContext = this;
                LoadApplicants();
            }

            private List<Resume> FetchApplicantsForCompany(int companyId)
            {
                var applicants = new List<Resume>();

                using (var connection = new MySqlConnection(_connection))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT * FROM resume_db WHERE CompanyId = @CompanyId";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@CompanyId", companyId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var resume = new Resume
                                {
                                    Applicantion_Id = Convert.ToInt32(reader["Profile_Number"]),
                                    CompanyID_Number = Convert.ToInt32(reader["CompanyId"]),
                                    Status = reader["Status"].ToString(),
                                    Resume_Number = Convert.ToInt32(reader["ResumeNumber"]),
                                    userProfile = reader["Profile_Name"].ToString(),
                                    Resume_Job_Position = reader["Position"].ToString(),
                                    Company_Name = reader["Company_Name"].ToString(),
                                    Submitted_Date = DateTime.Now,
                                    Email = reader["Email"].ToString()
                                };
                                applicants.Add(resume);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception instead of showing a MessageBox
                        Console.WriteLine($"Error fetching applicants: {ex.Message}");
                    }
                }

                return applicants;
            }

            private void LoadApplicants()
            {
                list = FetchApplicantsForCompany(MainWindow.CompanyID);
                Job_Sent_Table.ItemsSource = list;

                if (list.Count == 0)
                {
                    MessageBox.Show("No applicants found for the selected company.");
                }
            }

            private void ViewProfile(ApplicantUser user)
            {
                var profileWindow = new Applicant_Profile(user);
                profileWindow.Show();
            }

            private void Back_Btn_Click(object sender, RoutedEventArgs e)
            {
                var dashboard = new Company_DashBoard();
                this.Hide();
                dashboard.Show();
            }

            private void Submit_Btn(object sender, RoutedEventArgs e)
            {
                if (Job_Sent_Table.SelectedItem is Resume selectedUser)
                {
                    string recipientEmail = selectedUser.Email;
                    string subject = "From the hiring team";
                    string messageFilePath = "C:\\Users\\Admin\\source\\repos\\NewWin\\UMHire_JSrepos\\UMHire_JSrepos\\Project_Polished_Version\\Company_Message.txt"; // Update with the path to your text file

                    try
                    {
                        // Validate user input
                        if (string.IsNullOrWhiteSpace(recipientEmail) || string.IsNullOrWhiteSpace(subject))
                        {
                            MessageBox.Show("Please fill in all fields before sending the email.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Read the message from the text file
                        if (!File.Exists(messageFilePath))
                        {
                            MessageBox.Show("Message file not found. Please ensure the file exists at the specified location.", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        string message = File.ReadAllText(messageFilePath);

                        // Validate message content
                        if (string.IsNullOrWhiteSpace(message))
                        {
                            MessageBox.Show("The message file is empty. Please provide content in the file.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // SMTP configuration
                        string smtpServer = "smtp.gmail.com";
                        int smtpPort = 587; // TLS port
                        string senderEmail = "DaleDonalds31@gmail.com"; // Replace with your Gmail address
                        string senderAppPassword = "yohc kvol hqht ubup"; // Use your Gmail App Password

                        // Create email
                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(senderEmail),
                            Subject = subject,
                            Body = message,
                            IsBodyHtml = false
                        };
                        mailMessage.To.Add(recipientEmail);

                        // Set up the SMTP client
                        var smptClient = new SmtpClient(smtpServer)
                        {
                            Port = smtpPort,
                            Credentials = new NetworkCredential(senderEmail, senderAppPassword),
                            EnableSsl = true
                        };
                        smptClient.Send(mailMessage);

                        MessageBox.Show("Email sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (SmtpException)
                    {
                        MessageBox.Show("Error: Please check your email credentials, SMTP configuration, or internet connection.", "SMTP Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Error: Please ensure the recipient's email address is correct.", "Format Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}\nPlease check your internet connection or firewall settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }
    }

