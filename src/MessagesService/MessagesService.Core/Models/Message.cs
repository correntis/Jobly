using MessagesService.Core.Enums;

namespace MessagesService.Core.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string ChatId { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
