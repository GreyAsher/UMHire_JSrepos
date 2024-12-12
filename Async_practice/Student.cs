using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Async_practice
{
    public class Student
    {
        public string First_name { get; set; }

        public int Id {get; set;}

        public string Last_name { get; set; }

        public ICollection<Student> studentList { get; set; }
    }
}
