using System.Collections;
using System.Linq.Expressions;
using UsersService.Domain.Entities.NoSQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IResumesRepository
    {
        Task AddAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken = default);
        Task DeleteAsync(string entityId, CancellationToken cancellationToken = default);
        Task<ResumeEntity> GetAsync(string entityId, CancellationToken cancellationToken = default);
        Task<ResumeEntity> GetByAsync<TValue>(Expression<Func<ResumeEntity, TValue>> field, TValue value, CancellationToken cancellationToken = default);
        Task UpdateAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken = default);
        Task UpdateByAsync<TValue>(string id, Expression<Func<ResumeEntity, TValue>> field, TValue value, CancellationToken cancellationToken = default)
            where TValue : IEnumerable;
    }
}