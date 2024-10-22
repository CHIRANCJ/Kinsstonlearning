using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CurrentBatchStartDate { get; set; }
        public DateTime CurrentBatchEndDate { get; set; }

        // New field for approval status
        public bool IsApproved { get; set; }

        // Foreign Key
        public int ProfessorId { get; set; }
        public User Professor { get; set; }

        // Navigation properties
    
    }
}
