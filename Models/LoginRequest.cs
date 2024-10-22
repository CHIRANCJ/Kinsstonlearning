using System.ComponentModel.DataAnnotations;

namespace KinstonLearning.Models
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        //[Required]
        //public string Role { get; set; }
    }
}
