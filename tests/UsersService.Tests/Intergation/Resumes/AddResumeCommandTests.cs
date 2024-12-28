using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UsersService.Application.Resumes.Commands.AddResume;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

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
            var command = GetCommand(Guid.Empty);

            // Act
            var act = async () => await Sender.Send(command);

            // Arrange
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private AddResumeCommand GetCommand(Guid userId)
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

        private async Task<Guid> FillDatabaseAsync()
        {
            var userEntity = GetUserEntity();

            using var scope = _factory.Services.CreateScope();

            var manager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
            var result = await manager.CreateAsync(userEntity, "strinG123!");

            if (result.Succeeded)
            {
                return userEntity.Id;
            }

            throw new Bogus.ValidationException(JsonSerializer.Serialize(result.Errors));
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
