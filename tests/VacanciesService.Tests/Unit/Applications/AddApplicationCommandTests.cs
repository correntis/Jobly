using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.Applications.Commands.AddApplicationCommand;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Tests.Unit.Applications
{
    public class AddApplicationCommandTests
    {
        private readonly Mock<ILogger<AddApplicationCommandHandler>> _loggerMock;

        public AddApplicationCommandTests()
        {
            _loggerMock = new Mock<ILogger<AddApplicationCommandHandler>>();
        }

        [Fact]
        public async Task ShouldAdd_WhenValid()
        {
            // Arrange
            var command = new AddApplicationCommand(Guid.NewGuid(), Guid.NewGuid());

            var mapperMock = new Mock<IMapper>();
            var readVacanciesRepositoryMock = new Mock<IReadVacanciesRepository>();
            var writeApplicationsRepositoryMock = new Mock<IWriteApplicationsRepository>();
            var usersServiceMock = new Mock<IUsersService>();

            var handler = new AddApplicationCommandHandler(
                _loggerMock.Object,
                mapperMock.Object,
                readVacanciesRepositoryMock.Object,
                writeApplicationsRepositoryMock.Object,
                usersServiceMock.Object);

            usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            readVacanciesRepositoryMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacancyEntity());
            mapperMock.Setup(m => m.Map<ApplicationEntity>(It.IsAny<AddApplicationCommand>()))
                .Returns(new ApplicationEntity());

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            writeApplicationsRepositoryMock.Verify(
                wa => wa.AttachVacancy(It.IsAny<VacancyEntity>()),
                Times.Once);

            writeApplicationsRepositoryMock.Verify(
                wa => wa.AddAsync(It.IsAny<ApplicationEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);

            writeApplicationsRepositoryMock.Verify(
                wa => wa.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenUserNotExist()
        {
            // Arrange
            var command = new AddApplicationCommand(Guid.NewGuid(), Guid.NewGuid());

            var usersServiceMock = new Mock<IUsersService>();

            var handler = new AddApplicationCommandHandler(
                _loggerMock.Object,
                null,
                null,
                null,
                usersServiceMock.Object);

            usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenVacancyNotExist()
        {
            // Arrange
            var command = new AddApplicationCommand(Guid.NewGuid(), Guid.NewGuid());

            var usersServiceMock = new Mock<IUsersService>();
            var readVacanciesRepositoryMock = new Mock<IReadVacanciesRepository>();

            var handler = new AddApplicationCommandHandler(
                _loggerMock.Object,
                null,
                readVacanciesRepositoryMock.Object,
                null,
                usersServiceMock.Object);

            usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            readVacanciesRepositoryMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((VacancyEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
