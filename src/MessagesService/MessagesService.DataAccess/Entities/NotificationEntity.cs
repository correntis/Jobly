using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessagesService.DataAccess.Entities
{
    public class NotificationEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Guid RecipientId { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
