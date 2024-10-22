using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.Models
{
    public class Module
    {
        [Key] // Specify primary key
        public int ModuleId { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
