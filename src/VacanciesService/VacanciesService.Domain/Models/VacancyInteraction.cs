namespace VacanciesService.Domain.Models
{
    public class VacancyInteraction
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }
        public int Type { get; set; }
    }
}
