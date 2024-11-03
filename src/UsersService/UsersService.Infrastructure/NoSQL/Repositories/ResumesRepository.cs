using MongoDB.Driver;
using System.Linq.Expressions;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;

namespace UsersService.Infrastructure.NoSQL.Repositories
{
    public class ResumesRepository : IResumesRepository
    {
        private readonly MongoDbContext _context;

        public ResumesRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken)
        {
            InsertOneOptions options = null;

            await _context.Resumes.InsertOneAsync(resumeEntity, options, cancellationToken);
        }

        public async Task UpdateAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, resumeEntity.Id);

            ReplaceOptions options = null;

            await _context.Resumes.ReplaceOneAsync(filter, resumeEntity, options, cancellationToken);
        }

        public async Task DeleteAsync(string entityId, CancellationToken cancellationToken)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, entityId);

            await _context.Resumes.DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<ResumeEntity> GetAsync(string entityId, CancellationToken cancellationToken)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, entityId);

            return await _context.Resumes
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ResumeEntity> GetByAsync<TValue>(
            Expression<Func<ResumeEntity, TValue>> field,
            TValue value,
            CancellationToken cancellationToken)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(field, value);

            return await _context.Resumes
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
