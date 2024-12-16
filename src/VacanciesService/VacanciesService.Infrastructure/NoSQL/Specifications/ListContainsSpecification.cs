using MongoDB.Driver;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using VacanciesService.Domain.Entities.NoSQL;

namespace VacanciesService.Infrastructure.NoSQL.Specifications
{
    public class ListContainsSpecification<TEntity> : Specification<TEntity>
        where TEntity : VacancyDetailsEntity
    {
        public ListContainsSpecification(
            Expression<Func<TEntity, IEnumerable<string>>> fieldSelector,
            IEnumerable<string> listFilters)
            : base(GetFilter(fieldSelector, listFilters))
        {
        }

        private static FilterDefinition<TEntity> GetFilter(
            Expression<Func<TEntity, IEnumerable<string>>> fieldSelector,
            IEnumerable<string> listFilters)
        {
            var fieldName = ((MemberExpression)fieldSelector.Body).Member.Name;
            var values = listFilters.Select(req =>
                new StringOrRegularExpression(new Regex(req, RegexOptions.IgnoreCase)));

            var filter =
                Builders<TEntity>.Filter.Exists(fieldName) &
                Builders<TEntity>.Filter.AnyStringIn(fieldSelector, values);

            return filter;
        }
    }
}
