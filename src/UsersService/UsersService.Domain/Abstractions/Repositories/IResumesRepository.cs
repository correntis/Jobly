using System.Linq.Expressions;
using UsersService.Domain.Entities.NoSQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IResumesRepository
    {
        Task AddAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken);
        Task DeleteAsync(string entityId, CancellationToken cancellationToken);
        Task<ResumeEntity> GetAsync(string entityId, CancellationToken cancellationToken);
        Task<ResumeEntity> GetByAsync<TValue>(Expression<Func<ResumeEntity, TValue>> field, TValue value, CancellationToken cancellationToken);
        Task UpdateAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken);
        Task UpdateByAsync<TValue>(string id, Expression<Func<ResumeEntity, object>> field, TValue value, CancellationToken cancellationToken)
            where TValue : IEnumerable<object>;
    }
}