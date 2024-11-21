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

            if (salaryFilter.Min is not null && salaryFilter.Max is not null)
            {
                filter &= Builders<VacancyDetailsEntity>.Filter.And(
                    Builders<VacancyDetailsEntity>.Filter.Lte(vd => vd.Salary.Min, salaryFilter.Max),
                    Builders<VacancyDetailsEntity>.Filter.Gte(vd => vd.Salary.Max, salaryFilter.Min));
            }
            else if (salaryFilter.Min is not null)
            {
                filter &= Builders<VacancyDetailsEntity>.Filter.Gte(vd => vd.Salary.Max, salaryFilter.Min);
            }
            else if (salaryFilter.Max is not null)
            {
                filter &= Builders<VacancyDetailsEntity>.Filter.Lte(vd => vd.Salary.Min, salaryFilter.Max);
            }

            return filter;
        }
    }
}
