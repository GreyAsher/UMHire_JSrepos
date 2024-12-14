using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Async_practice
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> StudentCollecdtions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        optionsBuilder.UseMySql("Server=localhost;Database=project_database;UserName=root;Password=Cedric1234%%",
                builder =>
                {
                    builder.ServerVersion(new Version(3, 1, 32), ServerType.MySql); // Specify MySQL version and type
                });
            using (var context = new ApplicationDbContext())
            {
                var studentsClass = new Student
                {
                    First_name = "J.K. Rowling",
                    Last_name = "Cool",
                    studentList = new List<Student>
        {
            new Student { First_name = "Hello" },
            new Student { First_name = "Hello" }
        }
                };

                context.StudentCollecdtions.Add(studentsClass);
                context.SaveChanges(); // Save changes to the database
            }
        }
    }
}
