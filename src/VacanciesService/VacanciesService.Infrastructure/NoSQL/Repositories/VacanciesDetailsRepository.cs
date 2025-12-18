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
            var options = new InsertOneOptions();

            await _context.VacanciesDetails.InsertOneAsync(entity, options, cancellationToken);
        }

        public async Task UpdateAsync(VacancyDetailsEntity entity, CancellationToken cancellationToken = default)
        {
            var options = new ReplaceOptions();
            var filter = Builders<VacancyDetailsEntity>.Filter.Eq(vd => vd.Id, entity.Id);

            await _context.VacanciesDetails.ReplaceOneAsync(filter, entity, options, cancellationToken);
        }

        public async Task UpdateByAsync<TValue>(
            string id,
            Expression<Func<VacancyDetailsEntity, object>> field,
            TValue value,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<VacancyDetailsEntity>.Filter.Eq(vd => vd.Id, id);
            var update = Builders<VacancyDetailsEntity>.Update.Set(field, value);
            var options = new UpdateOptions();

            if (value is null)
            {
                update = Builders<VacancyDetailsEntity>.Update.Unset(field);
            }

            await _context.VacanciesDetails.UpdateOneAsync(filter, update, options, cancellationToken);
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
            if (filter is null)
            {
                return new FilterDefinitionBuilder<VacancyDetailsEntity>().Empty;
            }

            Specification<VacancyDetailsEntity> specification = null;

            // Фильтр по Title (если нужно, можно добавить поиск в VacancyDetails, но сейчас Title в основном объекте Vacancy)
            // Title фильтруется на уровне приложения в GetFilteredVacanciesQueryHandler

            if (filter.Requirements is not null && filter.Requirements.Count != 0)
            {
                var reqSpec = new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Requirements, filter.Requirements);
                specification = specification is null ? reqSpec : specification.And(reqSpec);
            }

            if (filter.Skills is not null && filter.Skills.Count != 0)
            {
                var skillsSpec = new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Skills, filter.Skills);
                specification = specification is null ? skillsSpec : specification.And(skillsSpec);
            }

            if (filter.Tags is not null && filter.Tags.Count != 0)
            {
                var tagsSpec = new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Tags, filter.Tags);
                specification = specification is null ? tagsSpec : specification.And(tagsSpec);
            }

            if (filter.Responsibilities is not null && filter.Responsibilities.Count != 0)
            {
                var respSpec = new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Responsibilities, filter.Responsibilities);
                specification = specification is null ? respSpec : specification.And(respSpec);
            }

            if (filter.Benefits is not null && filter.Benefits.Count != 0)
            {
                var benefitsSpec = new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Benefits, filter.Benefits);
                specification = specification is null ? benefitsSpec : specification.And(benefitsSpec);
            }

            if (filter.Education is not null && filter.Education.Count != 0)
            {
                var eduSpec = new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Education, filter.Education);
                specification = specification is null ? eduSpec : specification.And(eduSpec);
            }

            if (filter.Technologies is not null && filter.Technologies.Count != 0)
            {
                var techSpec = new ListContainsSpecification<VacancyDetailsEntity>(vd => vd.Technologies, filter.Technologies);
                specification = specification is null ? techSpec : specification.And(techSpec);
            }

            if (filter.Languages is not null && filter.Languages.Count != 0)
            {
                var langSpec = new VacancyLanguagesSpecification(filter.Languages);
                specification = specification is null ? langSpec : specification.And(langSpec);
            }

            if (filter.Experience is not null)
            {
                var expSpec = new VacancyExperienceSpefication(filter.Experience);
                specification = specification is null ? expSpec : specification.And(expSpec);
            }

            if (filter.Salary is not null)
            {
                var salarySpec = new VacancySalarySpecification(filter.Salary);
                specification = specification is null ? salarySpec : specification.And(salarySpec);
            }

            return SpecificationBuilder.GetFilter(specification);
        }

        public async Task<List<string>> GetDistinctRequirementsAsync(CancellationToken cancellationToken = default)
        {
            var allDetails = await _context.VacanciesDetails
                .Find(Builders<VacancyDetailsEntity>.Filter.Empty)
                .Project(vd => vd.Requirements)
                .ToListAsync(cancellationToken);

            var distinctRequirements = allDetails
                .Where(req => req != null)
                .SelectMany(req => req)
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            return distinctRequirements;
        }

        public async Task<List<string>> GetDistinctSkillsAsync(CancellationToken cancellationToken = default)
        {
            var allDetails = await _context.VacanciesDetails
                .Find(Builders<VacancyDetailsEntity>.Filter.Empty)
                .Project(vd => vd.Skills)
                .ToListAsync(cancellationToken);

            var distinctSkills = allDetails
                .Where(skills => skills != null)
                .SelectMany(skills => skills)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            return distinctSkills;
        }

        public async Task<List<string>> GetDistinctTechnologiesAsync(CancellationToken cancellationToken = default)
        {
            var allDetails = await _context.VacanciesDetails
                .Find(Builders<VacancyDetailsEntity>.Filter.Empty)
                .Project(vd => vd.Technologies)
                .ToListAsync(cancellationToken);

            var distinctTechnologies = allDetails
                .Where(tech => tech != null)
                .SelectMany(tech => tech)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            return distinctTechnologies;
        }
    }
}
