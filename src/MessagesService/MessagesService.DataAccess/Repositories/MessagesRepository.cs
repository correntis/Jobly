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

        public async Task<List<MessageEntity>> GetManyBy<TValue>(
            Expression<Func<MessageEntity, TValue>> field,
            TValue value,
            CancellationToken token = default)
        {
            return await _context.Messages.Find(Eq(field, value)).ToListAsync(token);
        }

        public async Task<List<MessageEntity>> SearchContent(
            string searchingContent,
            CancellationToken token = default)
        {
            var filter = Builders<MessageEntity>.Filter.Regex(
                message => message.Content,
                new BsonRegularExpression(searchingContent, "i"));

            return await _context.Messages.Find(filter).ToListAsync(token);
        }
    }
}
