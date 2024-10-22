using System;

namespace KinstonLearning.DTOs
{
    public class EnrollRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
