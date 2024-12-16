using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using UsersService.Infrastructure.NoSQL.Providers;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Configuration;
using VacanciesService.Domain.Entities.NoSQL;

namespace VacanciesService.Infrastructure.NoSQL
{
    public class MongoDbContext : IMongoDbContext
    {
        public IMongoCollection<VacancyDetailsEntity> VacanciesDetails { get; set; }

        static MongoDbContext()
        {
            BsonSerializer.RegisterSerializationProvider(new GuidSerializationProvider());

            var convention = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(true),
            };

            ConventionRegistry.Register("DefaultConventions", convention, type => true);
        }

        public MongoDbContext(IOptions<MongoDbOptions> options)
        {
            var client = new MongoClient(options.Value.Url);

            var database = client.GetDatabase(options.Value.Database);

            VacanciesDetails = database.GetCollection<VacancyDetailsEntity>("vacancies_details");
        }
    }
}
