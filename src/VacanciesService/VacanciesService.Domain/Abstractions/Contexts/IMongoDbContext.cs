using MongoDB.Driver;
using VacanciesService.Domain.Entities.NoSQL;

namespace VacanciesService.Domain.Abstractions.Contexts
{
    public interface IMongoDbContext
    {
        IMongoCollection<VacancyDetailsEntity> VacanciesDetails { get; set; }
    }
}