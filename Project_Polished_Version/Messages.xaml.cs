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
    /// Interaction logic for Messages.xaml
    /// </summary>
    public partial class Messages : Window
    {
        public Messages()
        {
            InitializeComponent();
        }

        private void BackMessagesClickBtn(object sender, RoutedEventArgs e)
        {
            Applicant_DashBoard mn = new Applicant_DashBoard();
            this.Hide();
            mn.Show();
        }
    }
}
