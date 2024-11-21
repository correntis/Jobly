using MongoDB.Bson.Serialization.Attributes;

namespace VacanciesService.Domain.Entities.NoSQL
{
    public abstract class BaseMongoEntity
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
