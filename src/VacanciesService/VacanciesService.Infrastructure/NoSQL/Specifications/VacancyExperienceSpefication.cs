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
                var minFilter = Builders<VacancyDetailsEntity>.Filter.Or(
                    Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Experience.Min, false),
                    Builders<VacancyDetailsEntity>.Filter.Gte(vd => vd.Experience.Min, experienceLevelFilter.Min));

                var maxFilter = Builders<VacancyDetailsEntity>.Filter.Or(
                    Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Experience.Max, false),
                    Builders<VacancyDetailsEntity>.Filter.Lte(vd => vd.Experience.Max, experienceLevelFilter.Max));

                if(experienceLevelFilter.Min is not null && experienceLevelFilter.Max is not null)
                {
                    filter &= Builders<VacancyDetailsEntity>.Filter.And(minFilter, maxFilter);
                }
                else if(experienceLevelFilter.Min is not null)
                {
                    filter &= minFilter;
                }
                else if(experienceLevelFilter.Max is not null)
                {
                    filter &= maxFilter;
                }
            }

            return filter;
        }
    }
}
