using System;
using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.DTOs
{
    public class CreateCourseRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime CurrentBatchStartDate { get; set; }

        [Required]
        public DateTime CurrentBatchEndDate { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int ProfessorId { get; set; }
    }
}
