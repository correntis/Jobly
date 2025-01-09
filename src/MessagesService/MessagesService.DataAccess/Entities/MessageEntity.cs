using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessagesService.DataAccess.Entities
{
    public class MessageEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ChatId { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public int Type { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
