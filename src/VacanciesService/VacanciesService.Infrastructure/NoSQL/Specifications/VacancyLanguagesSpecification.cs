using MongoDB.Driver;
using System.Text.RegularExpressions;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Filters.VacancyDetails;

namespace VacanciesService.Infrastructure.NoSQL.Specifications
{
    public class VacancyLanguagesSpecification : Specification<VacancyDetailsEntity>
    {
        public VacancyLanguagesSpecification(IEnumerable<LanguageFilter> languageFilters)
            : base(GetFilter(languageFilters))
        {
        }

        private static FilterDefinition<VacancyDetailsEntity> GetFilter(IEnumerable<LanguageFilter> languageFilters)
        {
            var filters = new List<FilterDefinition<VacancyDetailsEntity>>();

            foreach (var language in languageFilters)
            {
                var languageFilter = Builders<LanguageEntity>.Filter.And(
                    Builders<LanguageEntity>.Filter.Regex(l => l.Name, new Regex(language.Name, RegexOptions.IgnoreCase)),
                    Builders<LanguageEntity>.Filter.Regex(l => l.Level, new Regex(language.Level, RegexOptions.IgnoreCase)));

                filters.Add(Builders<VacancyDetailsEntity>.Filter.ElemMatch(vc => vc.Languages, languageFilter));
            }

            var filter =
                Builders<VacancyDetailsEntity>.Filter.Exists(vc => vc.Languages) &
                Builders<VacancyDetailsEntity>.Filter.Or(filters);

            return filter;
        }
    }
}
