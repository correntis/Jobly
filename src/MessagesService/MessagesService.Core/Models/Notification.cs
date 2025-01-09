using MessagesService.Core.Enums;

namespace MessagesService.Core.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public Guid RecipientId { get; set; }
        public string Content { get; set; }
        public NotificationType Type { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
