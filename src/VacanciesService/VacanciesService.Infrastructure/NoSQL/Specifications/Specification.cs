using MongoDB.Driver;
using VacanciesService.Domain.Entities.NoSQL;

namespace VacanciesService.Infrastructure.NoSQL.Specifications
{
    public class Specification<TEntity>
        where TEntity : BaseMongoEntity
    {
        public FilterDefinition<TEntity> Filter { get; }

        public Specification(FilterDefinition<TEntity> filter)
        {
            Filter = filter;
        }

        public Specification<TEntity> And(Specification<TEntity> right)
        {
            if (Filter is null)
            {
                return right;
            }

            if (right is null)
            {
                return this;
            }

            return new Specification<TEntity>(Builders<TEntity>.Filter.And(Filter, right.Filter));
        }

        public Specification<TEntity> Or(Specification<TEntity> right)
        {
            if (Filter is null)
            {
                return right;
            }

            if (right is null)
            {
                return this;
            }

            return new Specification<TEntity>(Builders<TEntity>.Filter.Or(Filter, right.Filter));
        }
    }
}
