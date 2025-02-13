using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acceloka.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; }

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
