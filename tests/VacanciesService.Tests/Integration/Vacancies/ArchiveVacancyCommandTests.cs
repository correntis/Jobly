using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VacanciesService.Application.Vacancies.Commands.ArchiveVacancyCommand;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Infrastructure.SQL;

namespace VacanciesService.Tests.Integration.Vacancies
{
    public class ArchiveVacancyCommandTests : BaseIntegrationTest
    {
        private readonly IntegrationTestWebAppFactory _factory;

        public ArchiveVacancyCommandTests(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldArchive_WhenExist()
        {
            // Arrange
            var id = await FillDatabaseAsync();

            // Act
            var act = async () => await Sender.Send(new ArchiveVacancyCommand(id));

            // Assert
            await act.Should().NotThrowAsync();

            (await CheckIfArchivedAsync(id)).Should().Be(true);
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenVacancyNotExist()
        {
            // Arrange
            var command = new ArchiveVacancyCommand(Guid.NewGuid());

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private async Task<bool> CheckIfArchivedAsync(Guid id)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            var entity = await context.Vacancies.Where(v => v.Id == id).FirstOrDefaultAsync();

            if (entity is null)
            {
                return false;
            }

            return entity.Archived;
        }

        private async Task<Guid> FillDatabaseAsync()
        {
            var entity = GetVacancyEntity();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            await context.Vacancies.AddAsync(entity);
            await context.SaveChangesAsync();

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
    }
}
