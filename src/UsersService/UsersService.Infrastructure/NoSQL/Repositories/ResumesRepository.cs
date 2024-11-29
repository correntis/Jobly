using MongoDB.Driver;
using System.Collections;
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
            await _context.Resumes.InsertOneAsync(resumeEntity, cancellationToken: cancellationToken);
        }

        public async Task UpdateAsync(ResumeEntity resumeEntity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, resumeEntity.Id);

            await _context.Resumes.ReplaceOneAsync(filter, resumeEntity, cancellationToken: cancellationToken);
        }

        public async Task UpdateByAsync<TValue>(
            string id,
            Expression<Func<ResumeEntity, TValue>> field,
            TValue value,
            CancellationToken cancellationToken = default)
            where TValue : IEnumerable
        {
            var filter = Builders<ResumeEntity>.Filter.Eq(r => r.Id, id);
            var update = Builders<ResumeEntity>.Update.Set(field, value);

            if (value is null)
            {
                var fieldWithObjectParameter = Expression.Lambda<Func<ResumeEntity, object>>(
                    Expression.Convert(field.Body, typeof(object)), field.Parameters);

                update = Builders<ResumeEntity>.Update.Unset(fieldWithObjectParameter);
            }

            await _context.Resumes.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
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
