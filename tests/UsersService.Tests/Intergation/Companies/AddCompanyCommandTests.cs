using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UsersService.Application.Companies.Commands.AddCompanyCommand;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Intergation.Companies
{
    public class AddCompanyCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public AddCompanyCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldShouldNotThrowException_WhenCompanyValid()
        {
            // Arrange
            var userId = await SaveUserToDatabaseAsync();
            var command = GetCommand(userId);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenUserForCompanyNotExist()
        {
            // Arrange
            var command = GetCommand(Guid.Empty);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        public AddCompanyCommand GetCommand(Guid userId)
        {
            var faker = new Faker();

            return new AddCompanyCommand(
                userId,
                faker.Company.CompanyName(),
                faker.Address.City(),
                faker.Address.SecondaryAddress(),
                faker.Internet.Email(),
                faker.Company.CompanySuffix(),
                null);
        }

        private async Task<Guid> SaveUserToDatabaseAsync()
        {
            var userEntity = GetUserEntity();

            using (var scope = _factory.Services.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
                var result = await manager.CreateAsync(userEntity, "strinG123!");

                if (!result.Succeeded)
                {
                    throw new Bogus.ValidationException(JsonSerializer.Serialize(result.Errors));
                }
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
                PhoneNumber = faker.Phone.PhoneNumber("+### (##) ###-##-##"),
                Email = faker.Internet.Email(),
                PasswordHash = faker.Random.Hash(),
                CreatedAt = DateTime.UtcNow,
                UserName = faker.Internet.UserName(),
            };
        }
    }
}
