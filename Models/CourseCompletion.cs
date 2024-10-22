using System;
using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.Models
{
    public class CourseCompletion
    {
        [Key] // Specify primary key
        public int CompletionId { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; }

        public DateTime CompletionDate { get; set; } = DateTime.Now;
    }
}
