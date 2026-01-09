using System.Collections.Generic;

namespace Domain.Entities
{
    public class Notification
    {
        public string? NotificationId { get; set; } = null;
        public string? UserId { get; set; }  = null;
        public string? Body { get; set; } = null;
        public string? Title { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? User { get; set; } = null; // Propriedade de navegação
    }
}
