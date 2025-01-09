using MessagesService.DataAccess.Entities;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Abstractions
{
    public interface IChatsRepository
    {
        Task AddAsync(ChatEntity entity, CancellationToken token = default);
        Task DeleteOneByAsync<TValue>(Expression<Func<ChatEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<ChatEntity> GetOneBy<TValue>(Expression<Func<ChatEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<List<ChatEntity>> GetPageBy<TValue>(Expression<Func<ChatEntity, TValue>> field, TValue value, int pageIndex, int pageSize, CancellationToken token = default);
        Task SetByIdAsync<TValue>(string id, Expression<Func<ChatEntity, TValue>> field, TValue value, CancellationToken token = default);
    }
}