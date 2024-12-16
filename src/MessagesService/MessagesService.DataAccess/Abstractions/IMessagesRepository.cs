using MessagesService.DataAccess.Entities;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Abstractions
{
    public interface IMessagesRepository
    {
        Task AddAsync(MessageEntity entity, CancellationToken token = default);
        Task UpdateAsync(MessageEntity entity, CancellationToken token = default);
        Task DeleteManyByAsync<TValue>(Expression<Func<MessageEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task DeleteOneByAsync<TValue>(Expression<Func<MessageEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<List<MessageEntity>> GetManyBy<TValue>(Expression<Func<MessageEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<MessageEntity> GetOneBy<TValue>(Expression<Func<MessageEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<List<MessageEntity>> SearchContent(string searchingContent, CancellationToken token = default);
    }
}