using MessagesService.DataAccess.Configuration.Options;
using MessagesService.DataAccess.Configuration.Providers;
using MessagesService.DataAccess.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace MessagesService.DataAccess
{
    public class MongoContext
    {
        public IMongoCollection<NotificationEntity> Notifications { get; init; }
        public IMongoCollection<MessageEntity> Messages { get; init; }

        static MongoContext()
        {
            BsonSerializer.RegisterSerializationProvider(new GuidSerializationProvider());

            var conventions = new ConventionPack()
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(true),
            };

            ConventionRegistry.Register("DefaultConventions", conventions, type => true);
        }

        public MongoContext(IOptions<MongoOptions> options)
        {
            var client = new MongoClient(options.Value.Uri);
            var database = client.GetDatabase(options.Value.Database);

            Notifications = database.GetCollection<NotificationEntity>("notifications");
            Messages = database.GetCollection<MessageEntity>("messages");

            CreateNotificationsIndicies();
            CreateMessagesIndicies();
        }

        private void CreateNotificationsIndicies()
        {
            if(Notifications is null)
            {
                throw new NullReferenceException("Collection notifications is null");
            }

            var recipientSentAtIndex = new CreateIndexModel<NotificationEntity>(
                Builders<NotificationEntity>.IndexKeys
                    .Ascending(notification => notification.RecipientId)
                    .Descending(notification => notification.SentAt));

            var recipientStatusIndex = new CreateIndexModel<NotificationEntity>(
                Builders<NotificationEntity>.IndexKeys
                    .Ascending(notification => notification.RecipientId)
                    .Ascending(notification => notification.Status));

            Notifications.Indexes.CreateMany([
                recipientSentAtIndex,
                recipientStatusIndex]);
        }

        private void CreateMessagesIndicies()
        {
            if(Messages is null)
            {
                throw new NullReferenceException("Collection messages is null");
            }

            var userSentAtIndex = new CreateIndexModel<MessageEntity>(
                Builders<MessageEntity>.IndexKeys
                    .Ascending(message => message.RecipientId)
                    .Descending(message => message.SentAt));

            var applicationSentAtIndex = new CreateIndexModel<MessageEntity>(
                Builders<MessageEntity>.IndexKeys
                    .Ascending(message => message.ApplicationId)
                    .Descending(message => message.SentAt));

            var companySentAtIndex = new CreateIndexModel<MessageEntity>(
                Builders<MessageEntity>.IndexKeys
                    .Ascending(message => message.SenderId)
                    .Descending(message => message.SentAt));

            Messages.Indexes.CreateMany([
                userSentAtIndex,
                applicationSentAtIndex,
                companySentAtIndex]);
        }
    }
}
