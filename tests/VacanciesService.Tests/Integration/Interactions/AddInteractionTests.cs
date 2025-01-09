using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VacanciesService.Application.Interactions.Commands.AddInteraction;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Infrastructure.SQL;

namespace VacanciesService.Tests.Integration.Interactions
{
    public class AddInteractionTests : BaseIntegrationTest
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private readonly Mock<IUsersService> _usersServiceMock;

        public AddInteractionTests(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;
            _usersServiceMock = _factory.UsersServiceMock;
        }

        [Fact]
        public async Task ShouldAdd_WhenValid()
        {
            // Arrange
            var vacancyId = await FillDatabaseAsync();
            var command = new AddInteractionCommand(Guid.NewGuid(), vacancyId, 0);

            _usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var idAct = await Sender.Send(command);

            // Assert
            (await CheckInteractionExistence(idAct)).Should().BeTrue();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenUserNotExist()
        {
            // Arrange
            var vacancyId = await FillDatabaseAsync();
            var command = new AddInteractionCommand(Guid.NewGuid(), vacancyId, 0);

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
            var command = new AddInteractionCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private async Task<bool> CheckInteractionExistence(Guid interactionId)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            var entity = await context.Interactions.FirstOrDefaultAsync(i => i.Id == interactionId);

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
