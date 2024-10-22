using System;
using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.Models
{
    public class Enrollment
    {
        [Key] // Specify primary key
        public int EnrollmentId { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    }
}
