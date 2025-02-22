using MongoDB.Driver;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Repositories
{
    public abstract class BaseMongoRepository<TEntity>
    {
        protected FilterDefinition<TEntity> Eq<TValue>(
            Expression<Func<TEntity, TValue>> field,
            TValue value)
        {
            return Builders<TEntity>.Filter.Eq(field, value);
        }

        protected FilterDefinition<TEntity> And(
            FilterDefinition<TEntity> first,
            FilterDefinition<TEntity> second)
        {
            return Builders<TEntity>.Filter.And(first, second);
        }
    }
}
