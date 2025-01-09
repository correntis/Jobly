using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UsersService.Application.Users.Commands.UpdateUser;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Intergation.Users
{
    public class UpdateUserCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public UpdateUserCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldNotThrowException_WhenUserExist()
        {
            // Arrange
            var userId = await FillDatabaseAsync();
            var command = GetCommand(userId);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenUserNotExist()
        {
            // Arrange
            var command = GetCommand(Guid.Empty);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
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

        private UpdateUserCommand GetCommand(Guid userId)
        {
            var faker = new Faker();

            return new UpdateUserCommand(
                userId,
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Phone.PhoneNumber("+### (##) ###-##-##"));
        }

        private UserEntity GetUserEntity()
        {
            var faker = new Faker();

            return new UserEntity()
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                PhoneNumber = faker.Phone.PhoneNumber("+### ## #######"),
                Email = faker.Internet.Email(),
                CreatedAt = DateTime.UtcNow,
                UserName = faker.Internet.UserName(),
            };
        }
    }
}
