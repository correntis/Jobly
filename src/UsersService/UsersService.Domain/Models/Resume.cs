namespace UsersService.Domain.Models
{
    public class Resume
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Tags { get; set; }
        public List<JobExperience> JobExperiences { get; set; }
        public List<Education> Educations { get; set; }
        public List<Certification> Certifications { get; set; }
        public List<Project> Projects { get; set; }
        public List<Language> Languages { get; set; }
    }
}
