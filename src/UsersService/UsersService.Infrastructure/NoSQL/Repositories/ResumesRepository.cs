using MongoDB.Driver;
using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
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

        public async Task<List<ResumeEntity>> FilterForVacancy(
            List<string> skills,
            List<string> tags,
            List<LanguageEntity> languages,
            CancellationToken cancellationToken = default)
        {
            var filter = BuildVacancyFilter(skills, tags, languages);

            return await _context.Resumes
                .Find(filter)
                .ToListAsync(cancellationToken);
        }

        private FilterDefinition<ResumeEntity> BuildVacancyFilter(
            List<string> skills,
            List<string> tags,
            List<LanguageEntity> languages)
        {
            var filters = new List<FilterDefinition<ResumeEntity>>();

            if(skills != null && skills.Count != 0)
            {
                var values = skills.Select(skill => new StringOrRegularExpression(new Regex(skill, RegexOptions.IgnoreCase)));

                filters.Add(Builders<ResumeEntity>.Filter.AnyStringIn(resume => resume.Skills, values));
            }

            if(tags != null && tags.Count != 0)
            {
                var values = tags.Select(tags => new StringOrRegularExpression(new Regex(tags, RegexOptions.IgnoreCase)));

                filters.Add(Builders<ResumeEntity>.Filter.AnyStringIn(resume => resume.Tags, values));
            }

            if(languages != null && languages.Count != 0)
            {
                foreach(var language in languages)
                {
                    var languageFilter = Builders<LanguageEntity>.Filter.And(
                        Builders<LanguageEntity>.Filter.Regex(l => l.Name, new Regex(language.Name, RegexOptions.IgnoreCase)),
                        Builders<LanguageEntity>.Filter.Regex(l => l.Level, new Regex(language.Level, RegexOptions.IgnoreCase)));

                    filters.Add(Builders<ResumeEntity>.Filter.ElemMatch(vc => vc.Languages, languageFilter));
                }
            }

            if(filters.Count == 0)
            {
                return Builders<ResumeEntity>.Filter.Empty;
            }

            return Builders<ResumeEntity>.Filter.Or(filters);
        }
    }
}
