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
        public IMongoCollection<ChatEntity> Chats { get; init; }
        public IMongoCollection<TemplateEntity> Templates { get; init; }

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
            Chats = database.GetCollection<ChatEntity>("chats");
            Templates = database.GetCollection<TemplateEntity>("templates");

            CreateNotificationsIndicies();
            CreateMessagesIndicies();
        }

        public void SeedTemplates()
        {
            var templates = GetTemplates();
            var amount = Templates.CountDocuments(FilterDefinition<TemplateEntity>.Empty);

            if(amount == 0)
            {
                Templates.InsertManyAsync(templates);
            }
        }

        private TemplateEntity[] GetTemplates()
        {
            return [
                new TemplateEntity()
                {
                    NotificationEvent = "RegistrationEvent",
                    NotificationType = 0,
                    Template = "Dear {0}. Thank you for registration!",
                },
                new TemplateEntity()
                {
                   NotificationEvent = "ApplicationResponseEvent",
                   NotificationType = 1,
                   Template = "Dear user {0}. Your application status for vacancy {1} has been updated. Current status {2}",
                },
                new TemplateEntity()
                {
                   NotificationEvent = "LikedVacancyDeadlineEvent",
                   NotificationType = 2,
                   Template = "Your liked vacancy {0} is about to expires in {1} days",
                },
                new TemplateEntity()
                {
                   NotificationEvent = "RecomendVacancyEvent",
                   NotificationType = 3,
                   Template = "One new vacancy mathches your resume with {0}%. {1}",
                },
                new TemplateEntity()
                {
                   NotificationEvent = "ResumeViewEvent",
                   NotificationType = 4,
                   Template = "Your resume has beem viewed by company {0}",
                },];
        }

        private void CreateNotificationsIndicies()
        {
            if (Notifications is null)
            {
                throw new NullReferenceException("Collection notifications is null");
            }

            var recipientSentAtIndex = new CreateIndexModel<NotificationEntity>(
                Builders<NotificationEntity>.IndexKeys
                    .Ascending(notification => notification.RecipientId)
                    .Descending(notification => notification.CreatedAt));

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
            if (Messages is null)
            {
                throw new NullReferenceException("Collection messages is null");
            }

            var userSentAtIndex = new CreateIndexModel<MessageEntity>(
                Builders<MessageEntity>.IndexKeys
                    .Ascending(message => message.RecipientId)
                    .Descending(message => message.SentAt));

            var chatSentAtIndex = new CreateIndexModel<MessageEntity>(
                Builders<MessageEntity>.IndexKeys
                    .Ascending(message => message.ChatId)
                    .Descending(message => message.SentAt));

            var companySentAtIndex = new CreateIndexModel<MessageEntity>(
                Builders<MessageEntity>.IndexKeys
                    .Ascending(message => message.SenderId)
                    .Descending(message => message.SentAt));

            Messages.Indexes.CreateMany([
                userSentAtIndex,
                chatSentAtIndex,
                companySentAtIndex]);
        }

        private void CreateChatsIndicies()
        {
            if (Chats is null)
            {
                throw new NullReferenceException("Collection chats is null");
            }

            var userIdLastMessageAtIndex = new CreateIndexModel<ChatEntity>(
                Builders<ChatEntity>.IndexKeys
                    .Ascending(message => message.UserId)
                    .Descending(message => message.LastMessageAt));

            var companyIdLastMessageAtIndex = new CreateIndexModel<ChatEntity>(
                Builders<ChatEntity>.IndexKeys
                    .Ascending(message => message.CompanyId)
                    .Descending(message => message.LastMessageAt));

            Chats.Indexes.CreateMany([
                userIdLastMessageAtIndex,
                companyIdLastMessageAtIndex]);
        }
    }
}
