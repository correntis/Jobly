using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Auth.Commands.RegisterUserCommand;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

namespace UsersService.Tests.Intergation.Auth
{
    public class RegisterUserCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public RegisterUserCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldSaveAndReturnToken_WhenEmailNotExist()
        {
            // Arrange
            var command = GetCommand();

            // Act
            var token = await Sender.Send(command);

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
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                await context.Users.AddAsync(userEntity);
                await context.SaveChangesAsync();
            }
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
                PasswordHash = faker.Internet.Password(),
                CreatedAt = DateTime.Now,
            };
        }

        public RegisterUserCommand GetCommand()
        {
            var faker = new Faker();

            return new RegisterUserCommand(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Internet.Email(),
                faker.Internet.Password(),
                faker.PickRandom(BusinessRules.Roles.All));
        }
    }
}
