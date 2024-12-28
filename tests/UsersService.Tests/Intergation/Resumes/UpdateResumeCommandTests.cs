using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System.Text.Json;
using UsersService.Application.Resumes.Commands.UpdateResume;
using UsersService.Domain.Abstractions.Repositories;
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
            var command = GetCommand(ObjectId.GenerateNewId().ToString(), Guid.Empty);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private UpdateResumeCommand GetCommand(string resumeId, Guid userId)
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

        private async Task<(string, Guid)> FillDatabaseAsync()
        {
            var userEntity = GetUserEntity();
            var resumeEntity = new ResumeEntity();

            using (var scope = _factory.Services.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
                var result = await manager.CreateAsync(userEntity, "strinG123!");

                if (!result.Succeeded)
                {
                    throw new Bogus.ValidationException(JsonSerializer.Serialize(result.Errors));
                }

                var usersContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

                resumeEntity = GetResumeEntity(userEntity.Id);

                var resumesRepository = scope.ServiceProvider.GetRequiredService<IResumesRepository>();

                await resumesRepository.AddAsync(resumeEntity);
            }

            return (resumeEntity.Id, userEntity.Id);
        }

        private ResumeEntity GetResumeEntity(Guid userId)
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
                PhoneNumber = faker.Phone.PhoneNumber("+### (##) ###-##-##"),
                Email = faker.Internet.Email(),
                PasswordHash = faker.Random.Hash(),
                CreatedAt = DateTime.UtcNow,
                UserName = faker.Internet.UserName(),
            };
        }
    }
}
