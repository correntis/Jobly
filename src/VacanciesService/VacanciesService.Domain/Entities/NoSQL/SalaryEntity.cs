namespace VacanciesService.Domain.Entities.NoSQL
{
    public class SalaryEntity : SalaryBaseEntity
    {
        public OriginalSalaryEntity Original { get; set; }
    }
}
