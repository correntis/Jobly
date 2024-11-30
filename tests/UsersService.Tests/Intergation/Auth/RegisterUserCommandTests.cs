using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UsersService.Application.Auth.Commands.RegisterUserCommand;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Intergation.Auth
{
    public class RegisterUserCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public RegisterUserCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;

            CreateRolesAsync();
        }

        [Fact]
        public async Task ShouldSaveAndReturnToken_WhenEmailNotExist()
        {
            // Arrange
            var command = GetCommand();

            // Act
            var (userId, token) = await Sender.Send(command);

            // Assert
            token.Should().NotBeNull();
            token.AccessToken.Should().NotBeNullOrEmpty();
            token.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldThrowValidationException_WhenRequestInvalid()
        {
            // Arrange
            var command = new RegisterUserCommand(null, null, null, null, null);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<Domain.Exceptions.ValidationException>();
        }

        [Fact]
        public async Task ShouldThrowEntityAlreadyExistException_WhenEmailExist()
        {
            // Arrange
            var command = GetCommand();
            var userEnity = GetUserEntity();

            await FillDatabaseAsync(userEnity);

            command = command with { Email = userEnity.Email };

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        private async Task FillDatabaseAsync(UserEntity userEntity)
        {
            using var scope = _factory.Services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
            var userResult = await userManager.CreateAsync(userEntity, "strinG123!");

            if (!userResult.Succeeded)
            {
                throw new Bogus.ValidationException(JsonSerializer.Serialize(userResult.Errors));
            }
        }

        private async Task CreateRolesAsync()
        {
            using var scope = _factory.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
            var roles = new List<RoleEntity>()
            {
                new () { Name = "User" },
                new () { Name = "Company" },
            };

            foreach (var role in roles)
            {
                var roleResult = await roleManager.CreateAsync(role);

                if (!roleResult.Succeeded)
                {
                    throw new Bogus.ValidationException(JsonSerializer.Serialize(roleResult.Errors));
                }
            }
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
                PasswordHash = faker.Internet.Password(),
                CreatedAt = DateTime.UtcNow,
                UserName = faker.Internet.UserName(),
            };
        }

        private RegisterUserCommand GetCommand()
        {
            var faker = new Faker();

            return new RegisterUserCommand(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Internet.Email(),
                "strinG123!",
                BusinessRules.Roles.All.ToList());
        }
    }
}
