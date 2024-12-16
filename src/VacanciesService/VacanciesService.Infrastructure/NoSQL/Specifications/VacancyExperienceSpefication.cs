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

            if (experienceLevelFilter.Min is not null && experienceLevelFilter.Max is not null)
            {
                filter &= Builders<VacancyDetailsEntity>.Filter.And(
                    Builders<VacancyDetailsEntity>.Filter.Lte(vd => vd.Experience.Min, experienceLevelFilter.Max),
                    Builders<VacancyDetailsEntity>.Filter.Gte(vd => vd.Experience.Max, experienceLevelFilter.Min));
            }
            else if (experienceLevelFilter.Min is not null)
            {
                filter &= Builders<VacancyDetailsEntity>.Filter.Gte(vd => vd.Experience.Max, experienceLevelFilter.Min);
            }
            else if (experienceLevelFilter.Max is not null)
            {
                filter &= Builders<VacancyDetailsEntity>.Filter.Lte(vd => vd.Experience.Min, experienceLevelFilter.Max);
            }

            return filter;
        }
    }
}
