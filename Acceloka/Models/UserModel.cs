using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class UserModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
       
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [StringLength(255)]
        public string UserEmail { get; set; } = string.Empty;

        [StringLength(255)]
        public string UserPassword { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
