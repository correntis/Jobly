namespace VacanciesService.Domain.Entities.SQL
{
    public class VacancyEntity : BaseEntity
    {
        public string Title { get; set; }
        public string EmploymentType { get; set; }
        public Guid CompanyId { get; set; }
        public bool Archived { get; set; }
        public DateTime DeadlineAt { get; set; }
        public List<ApplicationEntity> Applications { get; set; }
    }
}
