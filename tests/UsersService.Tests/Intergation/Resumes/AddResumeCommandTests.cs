using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Resumes.Commands.AddResumeCommand;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

namespace UsersService.Tests.Intergation.Resumes
{
    public class AddResumeCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public AddResumeCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        private async Task ShouldNotThrowException_WhenResumeValid()
        {
            // Arrange
            var userId = await FillDatabaseAsync();
            var command = GetCommand(userId);

            // Act
            var act = async () => await Sender.Send(command);

            // Arrange
            await act.Should().NotThrowAsync();
        }

        [Fact]
        private async Task ShouldThrowEntityAlreasyExistsException_WhenResumeForUserExist()
        {
            // Arrange
            var userId = await FillDatabaseAsync();
            var command = GetCommand(userId);

            await Sender.Send(command);

            // Act
            var act = async () => await Sender.Send(command);

            // Arrange
            await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        [Fact]
        private async Task ShouldThrowEntityNotFoundException_WhenUserForResumeNotExist()
        {
            // Arrange
            var command = GetCommand(int.MaxValue);

            // Act
            var act = async () => await Sender.Send(command);

            // Arrange
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private AddResumeCommand GetCommand(int userId)
        {
            var faker = new Faker();

            var skills = Enumerable.Repeat(faker.Random.Utf16String(20), faker.Random.Number(20)).ToList();
            var tags = Enumerable.Repeat(faker.Random.Utf16String(20), faker.Random.Number(20)).ToList();

            return new AddResumeCommand(
                userId,
                faker.Commerce.ProductDescription(),
                faker.Commerce.ProductDescription(),
                skills,
                tags);
        }

        private async Task<int> FillDatabaseAsync()
        {
            var userEntity = GetUserEntity();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                await context.Users.AddAsync(userEntity);
                await context.SaveChangesAsync();
            }

            return userEntity.Id;
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
