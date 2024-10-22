using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KinstonLearning.Models
{
    public class User
    {
        [Key] // Specify primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensure auto-increment
        public int UserId { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        public string Role { get; set; } // Example: "Student" or "Professor"
        public bool IsActive { get; set; } = false;
        public bool IsApproved { get; set; } = false;

      //  public DateTime RegistrationDate { get; set; } = DateTime.Now;

        //// New fields for Professor registration
        //public string Qualification { get; set; } // e.g., PhD, MSc
        //public int ExperienceYears { get; set; } // Number of years of experience
        //public string Specialization { get; set; } // e.g., Computer Science, Mathematics
        //public string ContactNumber { get; set; } // Contact number for communication
    }
}
