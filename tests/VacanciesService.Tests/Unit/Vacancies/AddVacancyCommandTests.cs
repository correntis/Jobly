using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.Vacancies.Commands.AddVacancyCommand;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;

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
            var vacanciesContextMock = new Mock<IVacanciesWriteContext>();
            var usersServiceMock = new Mock<IUsersService>();

            var command = GetValidCommand();
            var vacancyEntity = GetVacancyEntityFromCommand(command);

            var handler = new AddVacancyCommandHandler(
                _loggerMock.Object,
                mapperMock.Object,
                vacanciesContextMock.Object,
                usersServiceMock.Object);

            usersServiceMock.Setup(us => us.IsCompanyExistsAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(true);
            vacanciesContextMock.Setup(v => v.Vacancies.AddAsync(vacancyEntity, CancellationToken.None))
                .Returns(ValueTask.FromResult((EntityEntry<VacancyEntity>)null));
            vacanciesContextMock.Setup(v => v.SaveChangesAsync(CancellationToken.None))
                .Returns(Task.FromResult(0));
            mapperMock.Setup(m => m.Map<VacancyEntity>(command)).Returns(vacancyEntity);

            // Act

            var act = handler.Handle(command, CancellationToken.None);

            // Assert

            vacancyEntity.Archived.Should().BeFalse();

            vacanciesContextMock.Verify(
                v => v.Vacancies.AddAsync(vacancyEntity, CancellationToken.None),
                Times.Once);

            vacanciesContextMock.Verify(v => v.SaveChangesAsync(CancellationToken.None), Times.Once);
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
