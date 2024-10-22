using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.DTOs
{
    public class CreateModuleRequest
    {
        public int CourseId { get; set; }
        public int ProfessorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }

}
