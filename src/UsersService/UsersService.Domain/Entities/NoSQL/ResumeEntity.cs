using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UsersService.Domain.Entities.NoSQL
{
    public class ResumeEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Tags { get; set; }
        public List<JobExpirienceEntity> JobExpiriences { get; set; }
        public List<EducationEntity> Educations { get; set; }
        public List<CertificationEntity> Certifications { get; set; }
        public List<ProjectEntity> Projects { get; set; }
        public List<LanguageEntity> Languages { get; set; }
    }
}
