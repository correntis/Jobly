namespace MessagesService.DataAccess.Repositories
{
    public class NotificationsRepository
    {
        private readonly MongoContext _context;

        public NotificationsRepository(MongoContext context)
        {
            _context = context;
        }
    }
}
