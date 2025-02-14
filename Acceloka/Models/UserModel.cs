using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acceloka.Models
{
    public class UserModel
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("user_name")]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Column("user_email")]
        [StringLength(255)]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        [Column("user_password")]
        [StringLength(255)]
        public string UserPassword { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
