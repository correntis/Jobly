using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Companies.Commands.DeleteCompanyCommand;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

namespace UsersService.Tests.Intergation.Companies
{
    public class DeleteCompanyCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public DeleteCompanyCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldNotThrowException_WhenCompanyExist()
        {
            // Arrange
            var companyId = await FillDatabaseAsync();
            var command = new DeleteCompanyCommand(companyId);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenCompanyNotExist()
        {
            // Arrange
            var command = new DeleteCompanyCommand(Guid.Empty);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private async Task<Guid> FillDatabaseAsync()
        {
            var userEntity = GetUserEntity();
            var companyEntity = GetCompanyEntity(userEntity);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                await context.Users.AddAsync(userEntity);
                await context.Companies.AddAsync(companyEntity);
                await context.SaveChangesAsync();
            }

            return companyEntity.Id;
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
            };
        }

        private CompanyEntity GetCompanyEntity(UserEntity userEntity)
        {
            var faker = new Faker();

            return new CompanyEntity()
            {
                UserId = userEntity.Id,
                Name = faker.Name.FirstName(),
                Description = faker.Company.Bs(),
                City = faker.Address.City(),
                Address = faker.Address.SecondaryAddress(),
                Email = faker.Internet.Email(),
                Phone = faker.Phone.PhoneNumber("+### (##) ###-##-##"),
                Type = faker.Company.CompanySuffix(),
                WebSite = faker.Internet.Url(),
                User = userEntity,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
