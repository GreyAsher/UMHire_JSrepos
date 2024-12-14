using System;
using System.Net.Mail;
using System.Net;
using System.Windows;

namespace Project_Polished_Version
{

    public partial class ComposeEmail : Window
    {
        public ComposeEmail()
        {
            InitializeComponent();
        }

        private void Send_Email_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve input fields
            string recipientEmail = RecipientTextBox.Text;
            string subject = SubjectTextBox.Text;
            string message = MessageTextBox.Text;

            try
            {
                // Validate user input
                if (string.IsNullOrWhiteSpace(recipientEmail) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show("Please fill in all fields before sending the email.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            catch (SmtpException smtpEx)
            {
                MessageBox.Show($"Error: Please check your email credentials, SMTP configuration or internet connection.");
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show($"Error: Please ensure the recipient's email address is correct.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}\nPlease check your internet connection or firewall settings.");
            }
        }
    }
}

