using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MessagesService.DataAccess.Repositories
{
    public class MessagesRepository : BaseMongoRepository<MessageEntity>, IMessagesRepository
    {
        private readonly MongoContext _context;

        public MessagesRepository(MongoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MessageEntity entity, CancellationToken token = default)
        {
            await _context.Messages.InsertOneAsync(entity, cancellationToken: token);
        }

        public async Task UpdateAsync(MessageEntity entity, CancellationToken token = default)
        {
            await _context.Messages.ReplaceOneAsync(Eq(msg => msg.Id, entity.Id), entity, cancellationToken: token);
        }

        public async Task SetByIdAsync<TValue>(
            string id,
            Expression<Func<MessageEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            var update = Builders<MessageEntity>.Update.Set(field, value);

            await _context.Messages.UpdateOneAsync(Eq(msg => msg.Id, id), update, cancellationToken: token);
        }

        public async Task DeleteOneByAsync<TValue>(
            Expression<Func<MessageEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            await _context.Messages.DeleteOneAsync(Eq(field, value), cancellationToken: token);
        }

        public async Task DeleteManyByAsync<TValue>(
            Expression<Func<MessageEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            await _context.Messages.DeleteManyAsync(Eq(field, value), cancellationToken: token);
        }

        public async Task<MessageEntity> GetOneBy<TValue>(
            Expression<Func<MessageEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            return await _context.Messages.Find(Eq(field, value)).FirstOrDefaultAsync(token);
        }

        public async Task<List<MessageEntity>> GetPageBy<TValue>(
            Expression<Func<MessageEntity, TValue>> field,
            TValue value,
            int pageIndex,
            int pageSize,
            CancellationToken token = default)
        {
            return await _context.Messages
                .Find(Eq(field, value))
                .SortByDescending(msg => msg.SentAt)
                .Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(token);
        }

        public async Task<List<MessageEntity>> SearchContentForApplication(
            Guid applicationId,
            string searchingContent,
            CancellationToken token = default)
        {
            var idFilter = Eq(msg => msg.ApplicationId, applicationId);
            var contentFilter = Builders<MessageEntity>.Filter.Regex(
                message => message.Content, new BsonRegularExpression(searchingContent, "i"));

            return await _context.Messages
                .Find(idFilter & contentFilter)
                .SortByDescending(msg => msg.SentAt)
                .ToListAsync(token);
        }
    }
}
