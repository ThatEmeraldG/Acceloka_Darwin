using System.ComponentModel.DataAnnotations;

namespace Acceloka.Models
{
    public class CreateUserRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
