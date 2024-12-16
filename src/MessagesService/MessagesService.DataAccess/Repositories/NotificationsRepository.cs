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

        public async Task<List<NotificationEntity>> GetManyBy<TValue>(
            Expression<Func<NotificationEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            return await _context.Notifications.Find(Eq(field, value)).ToListAsync(token);
        }
    }
}
