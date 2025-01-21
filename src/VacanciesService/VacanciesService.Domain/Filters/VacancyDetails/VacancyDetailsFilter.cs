namespace VacanciesService.Domain.Filters.VacancyDetails
{
    public class VacancyDetailsFilter
    {
        public List<string> Requirements { get; set; } = null;
        public List<string> Skills { get; set; } = null;
        public List<string> Tags { get; set; } = null;
        public List<string> Responsibilities { get; set; } = null;
        public List<string> Benefits { get; set; } = null;
        public List<string> Education { get; set; } = null;
        public List<string> Technologies { get; set; } = null;
        public List<LanguageFilter> Languages { get; set; } = null;
        public ExperienceLevelFilter Experience { get; set; } = null;
        public SalaryFilter Salary { get; set; } = null;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
