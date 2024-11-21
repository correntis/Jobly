namespace VacanciesService.Domain.Filters.VacancyDetails
{
    public class VacancyDetailsFilter
    {
        public List<string> Requirements { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Responsibilities { get; set; }
        public List<string> Benefitrs { get; set; }
        public List<string> Education { get; set; }
        public List<string> Technologies { get; set; }
        public List<LanguageFilter> Languages { get; set; }
        public ExperienceLevelFilter Experience { get; set; }
        public SalaryFilter Salary { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
