using Bogus;
using FluentAssertions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using VacanciesService.Application.Vacancies.Commands.ArchiveVacancyCommand;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Entities.SQL;

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
            var vacanciesContextMock = new Mock<IVacanciesWriteContext>();
            var backgroundServiceMock = new Mock<IBackgroundJobClient>();

            var id = Guid.NewGuid();
            var vacancyEntity = GetVacancyEntity(id);

            var vacancies = new List<VacancyEntity> { vacancyEntity }.AsQueryable();
            var dbSetMock = new Mock<DbSet<VacancyEntity>>();

            dbSetMock.As<IQueryable<VacancyEntity>>().Setup(m => m.Provider).Returns(vacancies.Provider);
            dbSetMock.As<IQueryable<VacancyEntity>>().Setup(m => m.Expression).Returns(vacancies.Expression);
            dbSetMock.As<IQueryable<VacancyEntity>>().Setup(m => m.ElementType).Returns(vacancies.ElementType);
            dbSetMock.As<IQueryable<VacancyEntity>>().Setup(m => m.GetEnumerator()).Returns(vacancies.GetEnumerator());
            dbSetMock.Setup(d => d.FirstOrDefaultAsync(It.IsAny<Expression<Func<VacancyEntity, bool>>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Expression<Func<VacancyEntity, bool>> predicate, CancellationToken _) =>
                         vacancies.FirstOrDefault(predicate.Compile()));

            vacanciesContextMock.Setup(c => c.Vacancies).Returns(dbSetMock.Object);

            vacanciesContextMock.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var handler = new ArchiveVacancyCommandHandler(
                _loggerMock.Object,
                vacanciesContextMock.Object,
                backgroundServiceMock.Object);

            // Act
            var act = handler.Handle(new ArchiveVacancyCommand(id), CancellationToken.None);

            // Assert
            vacancyEntity.Archived.Should().BeTrue();

            vacanciesContextMock.Verify(
                v => v.Vacancies.Update(vacancyEntity),
                Times.Once);

            vacanciesContextMock.Verify(v => v.SaveChangesAsync(CancellationToken.None), Times.Once);
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
