using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Companies.Commands.AddCompanyCommand;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

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
            var command = GetCommand(int.MaxValue);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        public AddCompanyCommand GetCommand(int userId)
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

        private async Task<int> SaveUserToDatabaseAsync()
        {
            var userEntity = GetUserEntity();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                await context.AddAsync(userEntity);
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
