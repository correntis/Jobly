using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Entities;
using MongoDB.Driver;

namespace MessagesService.DataAccess.Repositories
{
    public class TemplatesRepository : ITemplatesRepository
    {
        private readonly MongoContext _context;

        public TemplatesRepository(MongoContext context)
        {
            _context = context;
        }

        public async Task<string> GetTemplateByType(int notificationType)
        {
            var filter = Builders<TemplateEntity>.Filter.Eq(temp => temp.NotificationType, notificationType);

            return await _context.Templates
                .Find(filter)
                .Project(temp => temp.Template)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetTemplateByEvent(string eventName)
        {
            var filter = Builders<TemplateEntity>.Filter.Eq(temp => temp.NotificationEvent, eventName);

            return await _context.Templates
                .Find(filter)
                .Project(temp => temp.Template)
                .FirstOrDefaultAsync();
        }
    }
}
