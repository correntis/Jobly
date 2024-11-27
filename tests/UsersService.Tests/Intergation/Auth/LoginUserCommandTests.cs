using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Auth.Commands.LoginUserCommand;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

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

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                await context.Users.AddAsync(userEntity);
                await context.SaveChangesAsync();
            }

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
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                await context.Users.AddAsync(userEntity);
                await context.SaveChangesAsync();
            }
        }

        private UserEntity GetUserEntityFromCommand(LoginUserCommand command)
        {
            var faker = new Faker();

            return new UserEntity()
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                Phone = faker.Phone.PhoneNumber("+### (##) ###-##-##"),
                Type = faker.PickRandom(BusinessRules.Roles.All),
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                CreatedAt = DateTime.UtcNow,
            };
        }

        private LoginUserCommand GetCommand()
        {
            var faker = new Faker();

            return new LoginUserCommand(
                faker.Internet.Email(),
                faker.Internet.Password());
        }
    }
}
