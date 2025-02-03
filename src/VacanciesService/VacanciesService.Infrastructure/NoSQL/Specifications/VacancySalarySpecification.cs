using MongoDB.Driver;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Filters.VacancyDetails;

namespace VacanciesService.Infrastructure.NoSQL.Specifications
{
    public class VacancySalarySpecification : Specification<VacancyDetailsEntity>
    {
        public VacancySalarySpecification(SalaryFilter salaryFilter)
            : base(GetFilter(salaryFilter))
        {
        }

        private static FilterDefinition<VacancyDetailsEntity> GetFilter(SalaryFilter salaryFilter)
        {
            var filter = Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Salary);

            if (salaryFilter.Min is not null || salaryFilter.Max is not null)
            {
                var minFilter = Builders<VacancyDetailsEntity>.Filter.Or(
                    Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Salary.Min, false),
                    Builders<VacancyDetailsEntity>.Filter.Gte(vd => vd.Salary.Min, salaryFilter.Min));

                var maxFilter = Builders<VacancyDetailsEntity>.Filter.Or(
                    Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Salary.Max, false),
                    Builders<VacancyDetailsEntity>.Filter.Lte(vd => vd.Salary.Max, salaryFilter.Max));

                if(salaryFilter.Min is not null && salaryFilter.Max is not null)
                {
                    filter &= Builders<VacancyDetailsEntity>.Filter.And(minFilter, maxFilter);
                }
                else if(salaryFilter.Min is not null)
                {
                    filter &= minFilter;
                }
                else if(salaryFilter.Max is not null)
                {
                    filter &= maxFilter;
                }
            }

            return filter;
        }
    }
}
