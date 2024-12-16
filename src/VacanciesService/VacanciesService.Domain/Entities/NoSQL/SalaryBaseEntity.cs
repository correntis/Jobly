namespace VacanciesService.Domain.Entities.NoSQL
{
    public abstract class SalaryBaseEntity
    {
        public string Currency { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
