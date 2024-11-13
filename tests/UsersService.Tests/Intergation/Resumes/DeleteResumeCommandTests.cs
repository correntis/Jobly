using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using UsersService.Application.Resumes.Commands.DeleteResumeCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

namespace UsersService.Tests.Intergation.Resumes
{
    public class DeleteResumeCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public DeleteResumeCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        private async Task ShouldNotThrowException_WhenResumeExist()
        {
            // Arrange
            var resumeId = await FillDatabaseAsync();
            var command = new DeleteResumeCommand(resumeId);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        private async Task ShouldThrowEntityNotFoundException_WhemResumeNotExist()
        {
            // Arrange
            var command = new DeleteResumeCommand(ObjectId.GenerateNewId().ToString());

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private async Task<string> FillDatabaseAsync()
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

            return resumeEntity.Id;
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
                CreatedAt = DateTime.Now,
            };
        }
    }
}
