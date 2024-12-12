using MongoDB.Driver;
using VacanciesService.Domain.Entities.NoSQL;

namespace VacanciesService.Infrastructure.NoSQL.Specifications
{
    public class SpecificationBuilder
    {
        public static FilterDefinition<TEntity> GetFilter<TEntity>(Specification<TEntity> specification)
            where TEntity : BaseMongoEntity
        {
            if (specification is not null && specification.Filter is not null)
            {
                return specification.Filter;
            }

            return new FilterDefinitionBuilder<TEntity>().Empty;
        }
    }
}
