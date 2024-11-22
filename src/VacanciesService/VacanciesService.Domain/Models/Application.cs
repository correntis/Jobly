namespace VacanciesService.Domain.Models
{
    public class Application
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public DateTime AppliedAt { get; set; }
        public string Status { get; set; }
        public Vacancy Vacancy { get; set; }
    }
}
