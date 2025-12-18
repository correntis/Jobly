using MongoDB.Driver;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Filters.VacancyDetails;

namespace VacanciesService.Infrastructure.NoSQL.Specifications
{
    public class VacancyExperienceSpefication : Specification<VacancyDetailsEntity>
    {
        public VacancyExperienceSpefication(ExperienceLevelFilter experienceLevelFilter)
            : base(GetFilter(experienceLevelFilter))
        {
        }

        private static FilterDefinition<VacancyDetailsEntity> GetFilter(ExperienceLevelFilter experienceLevelFilter)
        {
            var filter = Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Experience);

            if(experienceLevelFilter.Min is not null || experienceLevelFilter.Max is not null)
            {
                // Логика пересечения диапазонов:
                // Вакансия [vacancyMin, vacancyMax] пересекается с фильтром [filterMin, filterMax]
                // если: vacancyMin <= filterMax И vacancyMax >= filterMin
                
                var filters = new List<FilterDefinition<VacancyDetailsEntity>>();

                if(experienceLevelFilter.Min is not null)
                {
                    // Если фильтр требует минимум X лет, то вакансия должна принимать кандидатов с опытом >= X
                    // Это значит: либо у вакансии нет Max (принимает всех), либо Max вакансии >= Min фильтра
                    var minFilter = Builders<VacancyDetailsEntity>.Filter.Or(
                        Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Experience.Max, false),
                        Builders<VacancyDetailsEntity>.Filter.Gte(vd => vd.Experience.Max, experienceLevelFilter.Min));
                    filters.Add(minFilter);
                }

                if(experienceLevelFilter.Max is not null)
                {
                    // Если фильтр требует максимум X лет, то вакансия должна принимать кандидатов с опытом <= X
                    // Это значит: либо у вакансии нет Min (принимает всех), либо Min вакансии <= Max фильтра
                    var maxFilter = Builders<VacancyDetailsEntity>.Filter.Or(
                        Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Experience.Min, false),
                        Builders<VacancyDetailsEntity>.Filter.Lte(vd => vd.Experience.Min, experienceLevelFilter.Max));
                    filters.Add(maxFilter);
                }

                if(filters.Count > 0)
                {
                    filter &= Builders<VacancyDetailsEntity>.Filter.And(filters);
                }
            }

            return filter;
        }
    }
}
