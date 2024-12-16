namespace VacanciesService.Domain.Models
{
    public class Vacancy
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string EmploymentType { get; set; }
        public Guid CompanyId { get; set; }
        public bool Archived { get; set; }
        public DateTime DeadlineAt { get; set; }
        public VacancyDetails VacancyDetails { get; set; }
    }
}
