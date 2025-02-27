using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string UserPassword { get; set; } = string.Empty;
    }
}
