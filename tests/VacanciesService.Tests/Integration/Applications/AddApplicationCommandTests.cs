using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VacanciesService.Application.Applications.Commands.AddApplicationCommand;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Infrastructure.SQL;

namespace VacanciesService.Tests.Integration.Applications
{
    public class AddApplicationCommandTests : BaseIntegrationTest
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private readonly Mock<IUsersService> _usersServiceMock;

        public AddApplicationCommandTests(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
            _usersServiceMock = factory.UsersServiceMock;
        }

        [Fact]
        public async Task ShouldAdd_WhenValid()
        {
            // Arrange
            var vacancyId = await FillDatabaseAsync();
            var command = GetCommand(Guid.NewGuid(), vacancyId);

            _usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var idAct = await Sender.Send(command);

            // Assert
            (await CheckApplicationExistence(idAct)).Should().Be(true);
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenUserNotExist()
        {
            // Arrange
            var command = GetCommand(Guid.NewGuid(), Guid.NewGuid());

            _usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenVacancyNotExist()
        {
            // Arrange
            var command = GetCommand(Guid.NewGuid(), Guid.NewGuid());

            _usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        public AddApplicationCommand GetCommand(Guid userId, Guid vacancyId)
        {
            return new AddApplicationCommand(userId, vacancyId);
        }

        private async Task<bool> CheckApplicationExistence(Guid applicaitonId)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            var entity = await context.Applications.FirstOrDefaultAsync(a => a.Id == applicaitonId);

            return entity is not null;
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
