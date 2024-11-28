using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Users.Commands.UpdateUserCommand;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

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

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                await context.Users.AddAsync(userEntity);
                await context.SaveChangesAsync();
            }

            return userEntity.Id;
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
                Phone = faker.Phone.PhoneNumber("+### (##) ###-##-##"),
                Type = faker.PickRandom(BusinessRules.Roles.All),
                Email = faker.Internet.Email(),
                PasswordHash = faker.Random.Hash(),
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
