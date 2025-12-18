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
                // Логика пересечения диапазонов:
                // Вакансия [vacancyMin, vacancyMax] пересекается с фильтром [filterMin, filterMax]
                // если: vacancyMin <= filterMax И vacancyMax >= filterMin
                
                var filters = new List<FilterDefinition<VacancyDetailsEntity>>();

                if(salaryFilter.Min is not null)
                {
                    // Если фильтр требует минимум X, то вакансия должна предлагать зарплату >= X
                    // Это значит: либо у вакансии нет Max (предлагает любую), либо Max вакансии >= Min фильтра
                    var minFilter = Builders<VacancyDetailsEntity>.Filter.Or(
                        Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Salary.Max, false),
                        Builders<VacancyDetailsEntity>.Filter.Gte(vd => vd.Salary.Max, salaryFilter.Min));
                    filters.Add(minFilter);
                }

                if(salaryFilter.Max is not null)
                {
                    // Если фильтр требует максимум X, то вакансия должна предлагать зарплату <= X
                    // Это значит: либо у вакансии нет Min (предлагает любую), либо Min вакансии <= Max фильтра
                    var maxFilter = Builders<VacancyDetailsEntity>.Filter.Or(
                        Builders<VacancyDetailsEntity>.Filter.Exists(vd => vd.Salary.Min, false),
                        Builders<VacancyDetailsEntity>.Filter.Lte(vd => vd.Salary.Min, salaryFilter.Max));
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
