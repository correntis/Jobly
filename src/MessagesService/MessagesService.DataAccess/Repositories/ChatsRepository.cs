using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Repositories
{
    public class ChatsRepository : BaseMongoRepository<ChatEntity>, IChatsRepository
    {
        private readonly MongoContext _context;

        public ChatsRepository(MongoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChatEntity entity, CancellationToken token = default)
        {
            await _context.Chats.InsertOneAsync(entity, cancellationToken: token);
        }

        public async Task SetByIdAsync<TValue>(
            string id,
            Expression<Func<ChatEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            var update = Builders<ChatEntity>.Update.Set(field, value);

            await _context.Chats.UpdateOneAsync(Eq(msg => msg.Id, id), update, cancellationToken: token);
        }

        public async Task DeleteOneByAsync<TValue>(
            Expression<Func<ChatEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            await _context.Chats.DeleteOneAsync(Eq(field, value), cancellationToken: token);
        }

        public async Task<ChatEntity> GetOneBy<TValue>(
            Expression<Func<ChatEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            return await _context.Chats.Find(Eq(field, value)).FirstOrDefaultAsync(token);
        }

        public async Task<List<ChatEntity>> GetPageBy<TValue>(
            Expression<Func<ChatEntity, TValue>> field,
            TValue value,
            int pageIndex,
            int pageSize,
            CancellationToken token = default)
        {
            return await _context.Chats
                .Find(Eq(field, value))
                .SortByDescending(msg => msg.LastMessageAt)
                .Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(token);
        }
    }
}
