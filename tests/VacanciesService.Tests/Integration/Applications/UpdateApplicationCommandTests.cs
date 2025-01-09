using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VacanciesService.Application.Applications.Commands.UpdateApplication;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Infrastructure.SQL;

namespace VacanciesService.Tests.Integration.Applications
{
    public class UpdateApplicationCommandTests : BaseIntegrationTest
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private readonly Mock<IUsersService> _usersServiceMock;

        public UpdateApplicationCommandTests(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
            _usersServiceMock = factory.UsersServiceMock;
        }

        [Fact]
        public async Task ShouldUpdate_WhenValid()
        {
            // Arrange
            var applicationId = await FillDatabaseAsync();
            var command = new UpdateApplicationCommand(applicationId, "status");

            // Act
            var idAct = await Sender.Send(command);

            // Assert
            (await CheckApplicationStatus(idAct, command.Status)).Should().Be(true);
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenApplicationNotExist()
        {
            // Arrange
            var command = new UpdateApplicationCommand(Guid.NewGuid(), "status");

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private async Task<bool> CheckApplicationStatus(Guid id, string status)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            var entity = await context.Applications.FirstOrDefaultAsync(a => a.Id == id && a.Status == status);

            return entity is not null;
        }

        private async Task<Guid> FillDatabaseAsync()
        {
            var vacancyEntity = GetVacancyEntity();
            var applicationEntity = GetApplicationEntity();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            applicationEntity.Vacancy = vacancyEntity;

            await context.Applications.AddAsync(applicationEntity);
            await context.SaveChangesAsync();

            return applicationEntity.Id;
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

        private ApplicationEntity GetApplicationEntity()
        {
            var faker = new Faker("ru");

            return new ApplicationEntity()
            {
                UserId = Guid.NewGuid(),
                CreatedAt = faker.Date.Past(),
                Status = faker.Lorem.Word(),
            };
        }
    }
}
