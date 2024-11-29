namespace VacanciesService.Domain.Entities.NoSQL
{
    public class VacancyDetailsEntity : BaseMongoEntity
    {
        public Guid VacancyId { get; set; }
        public List<string> Requirements { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Responsibilities { get; set; }
        public List<string> Benefits { get; set; }
        public List<string> Education { get; set; }
        public List<string> Technologies { get; set; }
        public List<LanguageEntity> Languages { get; set; }
        public ExperienceLevelEntity Experience { get; set; }
        public SalaryEntity Salary { get; set; }
    }
}
