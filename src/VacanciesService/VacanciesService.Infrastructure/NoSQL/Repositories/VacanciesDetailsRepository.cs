using MongoDB.Driver;
using System.Linq.Expressions;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Infrastructure.NoSQL.Specifications;

namespace VacanciesService.Infrastructure.NoSQL.Repositories
{
    public class VacanciesDetailsRepository : IVacanciesDetailsRepository
    {
        private readonly IMongoDbContext _context;

        public VacanciesDetailsRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VacancyDetailsEntity entity, CancellationToken cancellationToken = default)
        {
            await _context.VacanciesDetails.InsertOneAsync(entity, null, cancellationToken);
        }

        public async Task UpdateAsync(VacancyDetailsEntity entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<VacancyDetailsEntity>.Filter.Eq(vd => vd.Id, entity.Id);

            await _context.VacanciesDetails.ReplaceOneAsync(filter, entity, (ReplaceOptions)null, cancellationToken);
        }

        public async Task UpdateByAsync<TValue>(
            string id,
            Expression<Func<VacancyDetailsEntity, object>> field,
            TValue value,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<VacancyDetailsEntity>.Filter.Eq(vd => vd.Id, id);
            var update = Builders<VacancyDetailsEntity>.Update.Set(field, value);

            if (value is null)
            {
                update = Builders<VacancyDetailsEntity>.Update.Unset(field);
            }

            await _context.VacanciesDetails.UpdateOneAsync(filter, update, null, cancellationToken);
        }

        public async Task DeleteByAsync<TValue>(
            Expression<Func<VacancyDetailsEntity, TValue>> field,
            TValue value,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<VacancyDetailsEntity>.Filter.Eq(field, value);

            await _context.VacanciesDetails.DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<VacancyDetailsEntity> GetByAsync<TValue>(
            Expression<Func<VacancyDetailsEntity, TValue>> field,
            TValue value,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<VacancyDetailsEntity>.Filter.Eq(field, value);

            return await _context.VacanciesDetails.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<VacancyDetailsEntity>> GetManyByAsync<TValue>(
            Expression<Func<VacancyDetailsEntity, TValue>> field,
            IEnumerable<TValue> values,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<VacancyDetailsEntity>.Filter.In(field, values);

            return await _context.VacanciesDetails.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<List<VacancyDetailsEntity>> GetFilteredPageAsync(
            VacancyDetailsFilter detailsFilter,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var filter = GetFilterFromSpecifications(detailsFilter);

            return await _context.VacanciesDetails
                .Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<VacancyDetailsEntity>> GetFilteredAsync(
            VacancyDetailsFilter detailsFilter,
            CancellationToken cancellationToken = default)
        {
            var filter = GetFilterFromSpecifications(detailsFilter);

            return await _context.VacanciesDetails
                .Find(filter)
                .ToListAsync(cancellationToken);
        }

        private FilterDefinition<VacancyDetailsEntity> GetFilterFromSpecifications(VacancyDetailsFilter filter)
        {
            var specification = new Specification<VacancyDetailsEntity>(null);

            if (filter is null)
            {
                return SpecificationBuilder.GetFilter(specification);
            }

            if (filter.Requirements is not null)
            {
                specification = specification.Or(
                    new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Requirements, filter.Requirements));
            }

            if (filter.Skills is not null)
            {
                specification = specification.Or(
                    new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Skills, filter.Skills));
            }

            if (filter.Tags is not null)
            {
                specification = specification.Or(
                    new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Tags, filter.Tags));
            }

            if (filter.Responsibilities is not null)
            {
                specification = specification.Or(
                    new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Responsibilities, filter.Responsibilities));
            }

            if (filter.Benefitrs is not null)
            {
                specification = specification.Or(
                    new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Benefits, filter.Benefitrs));
            }

            if (filter.Education is not null)
            {
                specification = specification.Or(
                    new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Education, filter.Education));
            }

            if (filter.Technologies is not null)
            {
                specification = specification.Or(
                    new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Technologies, filter.Technologies));
            }

            if (filter.Languages is not null)
            {
                specification = specification.Or(new VacancyLanguagesSpecification(filter.Languages));
            }

            if (filter.Experience is not null)
            {
                specification = specification.Or(new VacancyExperienceSpefication(filter.Experience));
            }

            if (filter.Salary is not null)
            {
                specification = specification.Or(new VacancySalarySpecification(filter.Salary));
            }

            return SpecificationBuilder.GetFilter(specification);
        }
    }
}
