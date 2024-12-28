using Bogus;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.Vacancies.Commands.ArchiveVacancy;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Tests.Unit.Vacancies
{
    public class ArchiveVacancyCommandTests
    {
        private readonly Mock<ILogger<ArchiveVacancyCommandHandler>> _loggerMock;

        public ArchiveVacancyCommandTests()
        {
            _loggerMock = new Mock<ILogger<ArchiveVacancyCommandHandler>>();
        }

        [Fact]
        public void ShouldAdd_WhenVacancyValid()
        {
            // Arrange
            var readVacanciesReposMock = new Mock<IReadVacanciesRepository>();
            var writeVacanciesReposMock = new Mock<IWriteVacanciesRepository>();
            var backgroundServiceMock = new Mock<IBackgroundJobClient>();

            var id = Guid.NewGuid();
            var vacancyEntity = GetVacancyEntity(id);

            readVacanciesReposMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(vacancyEntity);
            writeVacanciesReposMock.Setup(wr => wr.Update(vacancyEntity));
            writeVacanciesReposMock.Setup(wr => wr.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);

            var handler = new ArchiveVacancyCommandHandler(
                _loggerMock.Object,
                readVacanciesReposMock.Object,
                writeVacanciesReposMock.Object,
                backgroundServiceMock.Object);

            // Act
            var act = handler.Handle(new ArchiveVacancyCommand(id), CancellationToken.None);

            // Assert
            vacancyEntity.Archived.Should().BeTrue();

            readVacanciesReposMock.Verify(v => v.GetAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Once);
            writeVacanciesReposMock.Verify(wr => wr.Update(vacancyEntity), Times.Once);
            writeVacanciesReposMock.Verify(v => v.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public void ShouldThrowEntityNotFound_WhenCompanyNotExists()
        {
            // Arrange
            var readVacanciesReposMock = new Mock<IReadVacanciesRepository>();
            var writeVacanciesReposMock = new Mock<IWriteVacanciesRepository>();
            var backgroundServiceMock = new Mock<IBackgroundJobClient>();

            var id = Guid.NewGuid();

            readVacanciesReposMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync((VacancyEntity)null);

            var handler = new ArchiveVacancyCommandHandler(
                _loggerMock.Object,
                readVacanciesReposMock.Object,
                writeVacanciesReposMock.Object,
                backgroundServiceMock.Object);

            // Act
            var act = async () => await handler.Handle(new ArchiveVacancyCommand(id), CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<EntityNotFoundException>();
        }

        private VacancyEntity GetVacancyEntity(Guid id)
        {
            var faker = new Faker("ru");

            return new VacancyEntity()
            {
                Id = id,
                Title = faker.Name.JobTitle(),
                EmploymentType = faker.Name.JobType(),
                CompanyId = Guid.NewGuid(),
                DeadlineAt = faker.Date.Future(1),
            };
        }
    }
}
