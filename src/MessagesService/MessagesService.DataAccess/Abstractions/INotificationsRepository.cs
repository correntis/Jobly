using MessagesService.DataAccess.Entities;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Abstractions
{
    public interface INotificationsRepository
    {
        Task AddAsync(NotificationEntity entity, CancellationToken token = default);
        Task DeleteByAsync<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task DeleteManyByAsync<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<List<NotificationEntity>> GetManyBy<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
        Task<NotificationEntity> GetOneBy<TValue>(Expression<Func<NotificationEntity, TValue>> field, TValue value, CancellationToken token = default);
    }
}