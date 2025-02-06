using MessagesService.DataAccess.Entities;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Abstractions
{
    public interface INotificationsRepository
    {
        Task AddAsync(NotificationEntity entity, CancellationToken token = default);
        Task SetByIdAsync<TValue>(string id, Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task SetManyByIdsAsync<TValue>(List<string> ids, Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task DeleteByAsync<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task DeleteManyByAsync<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<List<NotificationEntity>> GetPageBy<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, int pageIndex, int pageSize, CancellationToken token = default);
        Task<List<NotificationEntity>> GetAllWithStatusBy<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, int status, CancellationToken token = default);
        Task<NotificationEntity> GetOneBy<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
    }
}