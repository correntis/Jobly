namespace VacanciesService.Domain.Models
{
    public abstract class SalaryBase
    {
        public string Currency { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
