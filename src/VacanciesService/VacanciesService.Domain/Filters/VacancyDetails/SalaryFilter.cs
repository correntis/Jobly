namespace VacanciesService.Domain.Filters.VacancyDetails
{
    public class SalaryFilter
    {
        public string Currency { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
