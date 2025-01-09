using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UsersService.Application.Auth.Commands.LoginUser;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Intergation.Auth
{
    public class LoginUserCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public LoginUserCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldReturnUserAndToken_WhenCredentialsValid()
        {
            // Arrange
            var command = GetCommand();
            var userEntity = GetUserEntityFromCommand(command);

            await FillDatabaseAsync(userEntity);

            // Act
            var (user, token) = await Sender.Send(command);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(userEntity.Id);
            user.Email.Should().Be(userEntity.Email);
            token.Should().NotBeNull();
            token.AccessToken.Should().NotBeNullOrEmpty();
            token.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldThrowInvalidPasswordException_WhenPasswordInvalid()
        {
            // Arrange
            var command = GetCommand();
            var userEntity = GetUserEntityFromCommand(command);

            command = command with { Password = string.Empty };

            await FillDatabaseAsync(userEntity);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<InvalidPasswordException>();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenEmailInvalid()
        {
            // Arrange
            var command = GetCommand();
            var userEntity = GetUserEntityFromCommand(command);

            command = command with { Email = string.Empty };

            await FillDatabaseAsync(userEntity);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private async Task FillDatabaseAsync(UserEntity userEntity)
        {
            using var scope = _factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
            var result = await manager.CreateAsync(userEntity, "strinG123!");

            if (!result.Succeeded)
            {
                throw new Bogus.ValidationException(JsonSerializer.Serialize(result.Errors));
            }
        }

        private UserEntity GetUserEntityFromCommand(LoginUserCommand command)
        {
            var faker = new Faker();

            return new UserEntity()
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                PhoneNumber = faker.Phone.PhoneNumber("+### (##) ###-##-##"),
                Email = command.Email,
                CreatedAt = DateTime.UtcNow,
                UserName = faker.Internet.UserName(),
            };
        }

        private LoginUserCommand GetCommand()
        {
            var faker = new Faker();

            return new LoginUserCommand(
                faker.Internet.Email(),
                "strinG123!");
        }
    }
}
