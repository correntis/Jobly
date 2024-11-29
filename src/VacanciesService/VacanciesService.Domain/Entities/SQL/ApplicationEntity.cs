namespace VacanciesService.Domain.Entities.SQL
{
    public class ApplicationEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public DateTime? AppliedAt { get; set; }
        public string Status { get; set; }
        public VacancyEntity Vacancy { get; set; }
    }
}
