using VacanciesService.Application.Abstractions;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Services
{
    public class VacancyTrainingDataConverter : IVacancyTrainingDataConverter
    {
        public string ConvertList(List<string> values)
        {
            return string.Join(",", values);
        }

        public string ConvertLanguages(List<LanguageEntity> languages)
        {
            return string.Join(",", languages.Select(l => $"{l.Name}-{l.Level}"));
        }

        public string ConvertLanguages(List<Language> languages)
        {
            return string.Join(",", languages.Select(l => $"{l.Name}-{l.Level}"));
        }

        public string ConvertExperience(ExperienceLevelEntity experience)
        {
            return $"{experience.Min}-{experience.Max}";
        }

        public string ConvertSalary(SalaryEntity salary)
        {
            return $"{BusinessRules.Salary.DefaultCurrency}:{salary.Min}-{salary.Max}";
        }
    }
}
