using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using UsersService.Application.Resumes.Commands.UpdateResumeCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

namespace UsersService.Tests.Intergation.Resumes
{
    public class UpdateResumeCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public UpdateResumeCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldNotThrowException_WhemResumeExist()
        {
            // Arrange
            var (resumeId, userId) = await FillDatabaseAsync();
            var command = GetCommand(resumeId, userId);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenResumeNotExist()
        {
            // Arrange
            var command = GetCommand(ObjectId.GenerateNewId().ToString(), int.MaxValue);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private UpdateResumeCommand GetCommand(string resumeId, int userId)
        {
            var faker = new Faker();

            var skills = Enumerable.Repeat(faker.Random.Utf16String(20), faker.Random.Number(20)).ToList();
            var tags = Enumerable.Repeat(faker.Random.Utf16String(20), faker.Random.Number(20)).ToList();

            return new UpdateResumeCommand(
                resumeId,
                userId,
                faker.Commerce.ProductDescription(),
                faker.Commerce.ProductDescription(),
                skills,
                tags);
        }

        private async Task<(string, int)> FillDatabaseAsync()
        {
            var userEntity = GetUserEntity();
            var resumeEntity = new ResumeEntity();

            using (var scope = _factory.Services.CreateScope())
            {
                var usersContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

                await usersContext.Users.AddAsync(userEntity);

                await usersContext.SaveChangesAsync();

                resumeEntity = GetResumeEntity(userEntity.Id);

                var resumesRepository = scope.ServiceProvider.GetRequiredService<IResumesRepository>();

                await resumesRepository.AddAsync(resumeEntity);
            }


            return (resumeEntity.Id, userEntity.Id);
        }

        private ResumeEntity GetResumeEntity(int userId)
        {
            var faker = new Faker();

            var skills = Enumerable.Repeat(faker.Random.Utf16String(20), faker.Random.Number(20)).ToList();
            var tags = Enumerable.Repeat(faker.Random.Utf16String(20), faker.Random.Number(20)).ToList();

            return new ResumeEntity()
            {
                UserId = userId,
                Title = faker.Commerce.ProductDescription(),
                Summary = faker.Commerce.ProductDescription(),
                Skills = skills,
                Tags = tags,
                CreatedAt = DateTime.UtcNow,
            };
        }

        private UserEntity GetUserEntity()
        {
            var faker = new Faker();

            return new UserEntity()
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                Phone = faker.Phone.PhoneNumber("+### (##) ###-##-##"),
                Type = faker.PickRandom(BusinessRules.Roles.All),
                Email = faker.Internet.Email(),
                PasswordHash = faker.Random.Hash(),
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
