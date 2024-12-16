using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessagesService.DataAccess.Entities
{
    public class MessageEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid VacancyId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime EditedAt { get; set; }
    }
}
