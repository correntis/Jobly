using Microsoft.ML.Data;

namespace VacanciesService.Domain.Models
{
    public class TrainingVacancyRecommendationData
    {
        [NoColumn]
        public Guid VacancyId { get; set; }
        [NoColumn]
        public Guid UserId { get; set; }

        public string ResumeSkills { get; set; }
        public string ResumeTags { get; set; }
        public string ResumeLanguages { get; set; }

        public string VacancySkills { get; set; }
        public string VacancyTags { get; set; }
        public string VacancyLanguages { get; set; }
        public string VacancyExperience { get; set; }
        public string VacancySalary { get; set; }

        public int InteractionType { get; set; }
        public float SuitabilityScore { get; set; }
    }
}
