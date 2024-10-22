using System;
using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.Models
{
    public class ModuleCompletion
    {
        [Key] // Specify primary key
        public int CompletionId { get; set; }

        public int ModuleId { get; set; }
        public Module Module { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; }

        public DateTime CompletionDate { get; set; } = DateTime.Now;
    }
}
