using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetails;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Models;
using VacanciesService.Infrastructure.NoSQL;
using VacanciesService.Infrastructure.SQL;

namespace VacanciesService.Tests.Integration.VacanciesDetails
{
    public class AddVacancyDetailsCommandTests : BaseIntegrationTest
    {
        private readonly IntegrationTestWebAppFactory _factory;

        public AddVacancyDetailsCommandTests(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldAdd_WhenDetailsValid()
        {
            // Arrange
            var vacancyId = await FillDatabaseWithVacancyAsync();
            var command = GetVacancyDetailsCommand(vacancyId);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ShouldThrowEntityAlreadyExist_WhenVacancyDetailsExist()
        {
            // Arrange
            var vacancyId = await FillDatabaseWithVacancyAndDetailsAsync();
            var command = GetVacancyDetailsCommand(vacancyId);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityAlreadyExistException>();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenVacancyNotExist()
        {
            // Arrange
            var command = GetVacancyDetailsCommand(Guid.NewGuid());

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private async Task<Guid> FillDatabaseWithVacancyAsync()
        {
            var entity = GetVacancyEntity();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            await context.Vacancies.AddAsync(entity);
            await context.SaveChangesAsync();

            return entity.Id;
        }

        private async Task<Guid> FillDatabaseWithVacancyAndDetailsAsync()
        {
            var entity = GetVacancyEntity();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            await context.Vacancies.AddAsync(entity);
            await context.SaveChangesAsync();

            var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            mongoContext.VacanciesDetails.InsertOne(new VacancyDetailsEntity() { VacancyId = entity.Id });

            return entity.Id;
        }

        private VacancyEntity GetVacancyEntity()
        {
            var faker = new Faker("ru");

            return new VacancyEntity()
            {
                Title = faker.Name.JobTitle(),
                EmploymentType = faker.Name.JobType(),
                CompanyId = Guid.NewGuid(),
                Archived = false,
                CreatedAt = DateTime.UtcNow,
                DeadlineAt = faker.Date.Future(1),
            };
        }

        private AddVacancyDetailsCommand GetVacancyDetailsCommand(Guid vacancyId)
        {
            var faker = new Faker("ru");

            return new AddVacancyDetailsCommand(
                vacancyId,
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Word()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Sentence()).ToList(),
                Enumerable.Range(1, 5).Select(_ => faker.Lorem.Word()).ToList(),
                Enumerable.Range(1, 3).Select(x => new Language()
                {
                    Level = faker.Lorem.Word(),
                    Name = faker.Lorem.Word(),
                }).ToList(),
                new ExperienceLevel()
                {
                    Min = faker.Random.Int(0, 3),
                    Max = faker.Random.Int(4, 10),
                },
                new Salary()
                {
                    Min = faker.Random.Int(0, 10000),
                    Max = faker.Random.Int(20000, 100000),
                    Currency = "USD",
                });
        }
    }
}
