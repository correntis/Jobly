using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UsersService.Domain.Entities.NoSQL
{
    public class ResumeEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Tags { get; set; }
        public List<JobExpirience> JobExpiriences { get; set; }
        public List<Education> Educations { get; set; }
        public List<Certification> Certifications { get; set; }
        public List<Project> Projects { get; set; }
        public List<Language> Languages { get; set; }
    }
}
