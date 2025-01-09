using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessagesService.DataAccess.Entities
{
    public class TemplateEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int NotificationType { get; set; }
        public string NotificationEvent { get; set; }
        public string Template { get; set; }
    }
}
