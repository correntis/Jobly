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

        public async Task AddAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken = default)
        {
            InsertOneOptions options = null;

            await _context.Resumes.InsertOneAsync(resumeEntity, options, cancellationToken);
        }

        public async Task UpdateAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, resumeEntity.Id);

            ReplaceOptions options = null;

            await _context.Resumes.ReplaceOneAsync(filter, resumeEntity, options, cancellationToken);
        }

        public async Task UpdateByAsync<TValue>(
            string id,
            Expression<Func<ResumeEntity, object>> field,
            TValue value,
            CancellationToken cancellationToken = default)
            where TValue : IEnumerable<object>
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, id);
            var update = Builders<ResumeEntity>.Update.Set(field, value);

            UpdateOptions options = null;

            if (value is null)
            {
                update = Builders<ResumeEntity>.Update.Unset(field);
            }

            await _context.Resumes.UpdateOneAsync(filter, update, options, cancellationToken);
        }

        public async Task DeleteAsync(string entityId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, entityId);

            await _context.Resumes.DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<ResumeEntity> GetAsync(string entityId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, entityId);

            return await _context.Resumes
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ResumeEntity> GetByAsync<TValue>(
            Expression<Func<ResumeEntity, TValue>> field,
            TValue value,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(field, value);

            return await _context.Resumes
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
