using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Polished_Version.Classes
{
    public class Resume
    {
        public string userProfile { get; set; }

        public int CompanyID_Number { get; set; }
        public string Company_Name { get; set; }
        public int Resume_Number { get; set; }

        public string Status { get; set; }

        public int Applicant_Number { get; set; }
        public DateTime Submitted_Date { get; set; }
        public int Applicantion_Id { get; set; }
        public string Email { get; set; }
        public string Resume_Job_Position { get; set; }
        public Resume() { }
    }
}
