namespace VacanciesService.Domain.Entities.SQL
{
    public class VacancyInteractionEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }
        public int Type { get; set; }
    }
}
