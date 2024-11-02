using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using UsersService.Domain.Configuration;
using UsersService.Domain.Entities.NoSQL;

namespace UsersService.Infrastructure.NoSQL
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public IMongoCollection<ResumeEntity> Resumes => _database.GetCollection<ResumeEntity>("resumes");

        static MongoDbContext()
        {
            var conventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(true),
            };

            ConventionRegistry.Register("DefaultConvetions", conventions, type => true);
        }

        public MongoDbContext(IOptions<MongoDbOptions> options)
        {
            var client = new MongoClient(options.Value.Url);

            _database = client.GetDatabase(options.Value.Database);
        }
    }
}
