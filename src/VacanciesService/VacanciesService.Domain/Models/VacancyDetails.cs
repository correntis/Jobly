namespace VacanciesService.Domain.Models
{
    public class VacancyDetails
    {
        public string Id { get; set; }
        public int VacancyId { get; set; }
        public List<string> Requirements { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Responsibilities { get; set; }
        public List<string> Benefits { get; set; }
        public List<string> Education { get; set; }
        public List<string> Technologies { get; set; }
        public List<Language> Languages { get; set; }
        public ExperienceLevel Experience { get; set; }
        public Salary Salary { get; set; }
    }
}
