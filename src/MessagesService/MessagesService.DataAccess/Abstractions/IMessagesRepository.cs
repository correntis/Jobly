using MessagesService.DataAccess.Entities;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Abstractions
{
    public interface IMessagesRepository
    {
        Task AddAsync(MessageEntity entity, CancellationToken token = default);
        Task UpdateAsync(MessageEntity entity, CancellationToken token = default);
        Task SetByIdAsync<TValue>(string id, Expression<Func<MessageEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task DeleteManyByAsync<TValue>(Expression<Func<MessageEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task DeleteOneByAsync<TValue>(Expression<Func<MessageEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<List<MessageEntity>> GetPageBy<TValue>(Expression<Func<MessageEntity, TValue>> field, TValue value, int pageIndex, int pageSize, CancellationToken token = default);
        Task<MessageEntity> GetOneBy<TValue>(Expression<Func<MessageEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<List<MessageEntity>> SearchContentForApplication(Guid applicationId, string searchingContent, CancellationToken token = default);
    }
}