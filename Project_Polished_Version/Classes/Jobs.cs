using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Polished_Version.Classes
{
    public class Jobs
    {
        public string Job_Description { get; set; }
        public int Job_id { get; set; }
        public string Job_Position { get; set; }
        public int Company_userNumber { get; set; }
        public string IsFilled { get; set; }
        public string Job_Company { get; set; }
        public Jobs()
        {
        }

        public Jobs(string jp, int ji, string jd, int cun, string isfilled, string J_Company)
        {
            Job_Position = jp;
            Job_id = ji;
            Job_Description = jd;
            Company_userNumber = cun;
            IsFilled = isfilled;
            Job_Company = J_Company;

        }
        public override string ToString()
        {
            return $" {Job_Company} - {Job_Position} - {Job_Description}";
        }
    }
}

