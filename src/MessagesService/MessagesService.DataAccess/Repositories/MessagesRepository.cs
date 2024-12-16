namespace MessagesService.DataAccess.Repositories
{
    public class MessagesRepository
    {
        private readonly MongoContext _context;

        public MessagesRepository(MongoContext context)
        {
            _context = context;
        }
    }
}
