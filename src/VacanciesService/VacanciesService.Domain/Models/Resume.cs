namespace VacanciesService.Domain.Models
{
    public class Resume
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Tags { get; set; }
        public List<Language> Languages { get; set; }
    }
}
