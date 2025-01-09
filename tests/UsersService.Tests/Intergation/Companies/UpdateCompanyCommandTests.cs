using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UsersService.Application.Companies.Commands.UpdateCompany;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Infrastructure.SQL;

namespace UsersService.Tests.Intergation.Companies
{
    public class UpdateCompanyCommandTests : BaseIntegrationTest
    {
        private readonly IntergationTestWebAppFactory _factory;

        public UpdateCompanyCommandTests(IntergationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldNotThrowException_WhenCompanyExist()
        {
            // Arrange
            var companyId = await FillDatabaseAsync();
            var command = GetCommand(companyId);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhemCompanyNotExist()
        {
            // Arrange
            var command = GetCommand(Guid.NewGuid());

            // Act
            var act = async () => await Sender.Send(command);

            // Arrange
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private UpdateCompanyCommand GetCommand(Guid companyId)
        {
            var faker = new Faker();

            return new UpdateCompanyCommand(
                companyId,
                faker.Company.CompanyName(),
                faker.Company.Bs(),
                faker.Address.City(),
                faker.Address.SecondaryAddress(),
                faker.Internet.Email(),
                faker.Phone.PhoneNumber("+### (##) ###-##-##"),
                faker.Internet.Url(),
                faker.Company.CompanySuffix(),
                null);
        }

        private async Task<Guid> FillDatabaseAsync()
        {
            var userEntity = GetUserEntity();
            var companyEntity = GetCompanyEntity(userEntity);

            using (var scope = _factory.Services.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
                var result = await manager.CreateAsync(userEntity, "strinG123!");

                if (!result.Succeeded)
                {
                    throw new Bogus.ValidationException(JsonSerializer.Serialize(result.Errors));
                }

                var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
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
                UserName = faker.Internet.UserName(),
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
