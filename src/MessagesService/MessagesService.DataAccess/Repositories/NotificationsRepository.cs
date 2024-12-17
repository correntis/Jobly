using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Repositories
{
    public class NotificationsRepository : BaseMongoRepository<NotificationEntity>, INotificationsRepository
    {
        private readonly MongoContext _context;

        public NotificationsRepository(MongoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NotificationEntity entity, CancellationToken token = default)
        {
            await _context.Notifications.InsertOneAsync(entity, cancellationToken: token);
        }

        public async Task SetByIdAsync<TValue>(
            string id,
            Expression<Func<NotificationEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            var update = Builders<NotificationEntity>.Update.Set(field, value);

            await _context.Notifications.UpdateOneAsync(Eq(notif => notif.Id, id), update, cancellationToken: token);
        }

        public async Task DeleteByAsync<TValue>(
            Expression<Func<NotificationEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            await _context.Notifications.DeleteOneAsync(Eq(field, value), cancellationToken: token);
        }

        public async Task DeleteManyByAsync<TValue>(
            Expression<Func<NotificationEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            await _context.Notifications.DeleteManyAsync(Eq(field, value), cancellationToken: token);
        }

        public async Task<NotificationEntity> GetOneBy<TValue>(
            Expression<Func<NotificationEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            return await _context.Notifications.Find(Eq(field, value)).FirstOrDefaultAsync(token);
        }

        public async Task<List<NotificationEntity>> GetPageBy<TValue>(
            Expression<Func<NotificationEntity, TValue>> field,
            TValue value,
            int pageIndex,
            int pageSize,
            CancellationToken token = default)
        {
            return await _context.Notifications
                .Find(Eq(field, value))
                .SortByDescending(notif => notif.SentAt)
                .Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(token);
        }
    }
}
