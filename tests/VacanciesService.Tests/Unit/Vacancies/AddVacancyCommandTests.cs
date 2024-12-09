using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.Vacancies.Commands.AddVacancyCommand;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Tests.Unit.Vacancies
{
    public class AddVacancyCommandTests
    {
        private readonly Mock<ILogger<AddVacancyCommandHandler>> _loggerMock;

        public AddVacancyCommandTests()
        {
            _loggerMock = new Mock<ILogger<AddVacancyCommandHandler>>();
        }

        [Fact]
        public void ShouldAdd_WhenVacancyValid()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var writeVacanciesReposMock = new Mock<IWriteVacanciesRepository>();
            var usersServiceMock = new Mock<IUsersService>();

            var command = GetValidCommand();
            var vacancyEntity = GetVacancyEntityFromCommand(command);

            var handler = new AddVacancyCommandHandler(
                _loggerMock.Object,
                mapperMock.Object,
                writeVacanciesReposMock.Object,
                usersServiceMock.Object);

            usersServiceMock.Setup(us => us.IsCompanyExistsAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(true);
            writeVacanciesReposMock.Setup(v => v.AddAsync(vacancyEntity, CancellationToken.None)).Returns(Task.CompletedTask);
            writeVacanciesReposMock.Setup(v => v.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));
            mapperMock.Setup(m => m.Map<VacancyEntity>(command)).Returns(vacancyEntity);

            // Act
            var act = handler.Handle(command, CancellationToken.None);

            // Assert
            vacancyEntity.Archived.Should().BeFalse();

            writeVacanciesReposMock.Verify(v => v.AddAsync(vacancyEntity, CancellationToken.None), Times.Once);
            writeVacanciesReposMock.Verify(v => v.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public void ShouldAdd_WhenCompanyNotExists()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var writeVacanciesReposMock = new Mock<IWriteVacanciesRepository>();
            var usersServiceMock = new Mock<IUsersService>();

            var command = new AddVacancyCommand(null, null, Guid.NewGuid(), DateTime.MinValue);

            var handler = new AddVacancyCommandHandler(
                _loggerMock.Object,
                mapperMock.Object,
                writeVacanciesReposMock.Object,
                usersServiceMock.Object);

            usersServiceMock.Setup(us => us.IsCompanyExistsAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(false);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private VacancyEntity GetVacancyEntityFromCommand(AddVacancyCommand command)
        {
            return new VacancyEntity()
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                EmploymentType = command.EmploymentType,
                CompanyId = command.CompanyId,
                DeadlineAt = command.DeadlineAt,
            };
        }

        private AddVacancyCommand GetValidCommand()
        {
            var faker = new Faker("ru");

            return new AddVacancyCommand(
                faker.Name.JobTitle(),
                faker.Name.JobType(),
                Guid.NewGuid(),
                faker.Date.Future(1));
        }
    }
}
