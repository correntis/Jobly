using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Abstractions
{
    public interface IVacancyTrainingDataConverter
    {
        string ConvertExperience(ExperienceLevelEntity experience);
        string ConvertLanguages(List<Language> languages);
        string ConvertLanguages(List<LanguageEntity> languages);
        string ConvertList(List<string> values);
        string ConvertSalary(SalaryEntity salary);
    }
}