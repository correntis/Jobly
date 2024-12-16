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
        }
    }
}
