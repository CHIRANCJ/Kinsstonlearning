using System;
using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.Models
{
    public class CourseChangeRequest
    {
        [Key] // Specify primary key
        public int ChangeRequestId { get; set; }

        [Required] // Mark as required if you want to enforce this
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required] // Ensure NewTitle is provided
        [StringLength(200)] // Optional: Set maximum length for title
        public string NewTitle { get; set; }

        [Required] // Ensure NewDescription is provided
        public string NewDescription { get; set; }

        [Required] // Mark as required if you want to enforce this
        public int RequestedByProfessorId { get; set; }
        public User RequestedByProfessor { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;
        public bool IsApplied { get; set; } = false;

        [Required] // Ensure this date is set
        public DateTime ProposedBatchStartDate { get; set; }

        [Required] // Ensure this date is set
        public DateTime ProposedBatchEndDate { get; set; }
    }
}
