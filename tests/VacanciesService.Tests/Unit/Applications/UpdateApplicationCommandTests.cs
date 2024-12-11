using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.Applications.Commands.UpdateApplicationCommand;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Tests.Unit.Applications
{
    public class UpdateApplicationCommandTests
    {
        private readonly Mock<ILogger<UpdateApplicationCommandHandler>> _loggerMock;

        public UpdateApplicationCommandTests()
        {
            _loggerMock = new Mock<ILogger<UpdateApplicationCommandHandler>>();
        }

        [Fact]
        public async Task ShouldUpdate_WhenValid()
        {
            // Arrange
            var command = new UpdateApplicationCommand(Guid.NewGuid(), "status");

            var mapperMock = new Mock<IMapper>();
            var readApplicationsRepositoryMock = new Mock<IReadApplicationsRepository>();
            var writeApplicationsRepositoryMock = new Mock<IWriteApplicationsRepository>();
            var readVacanciesRepositoryMock = new Mock<IReadVacanciesRepository>();

            var handler = new UpdateApplicationCommandHandler(
                _loggerMock.Object,
                readVacanciesRepositoryMock.Object,
                readApplicationsRepositoryMock.Object,
                writeApplicationsRepositoryMock.Object,
                mapperMock.Object);

            readApplicationsRepositoryMock.Setup(ra => ra.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationEntity());

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            writeApplicationsRepositoryMock.Verify(
                wa => wa.Update(It.IsAny<ApplicationEntity>()),
                Times.Once);

            writeApplicationsRepositoryMock.Verify(
                wa => wa.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenApplicationNotExist()
        {
            // Arrange
            var command = new UpdateApplicationCommand(Guid.NewGuid(), "status");

            var readApplicationsRepositoryMock = new Mock<IReadApplicationsRepository>();

            var handler = new UpdateApplicationCommandHandler(
                _loggerMock.Object,
                null,
                readApplicationsRepositoryMock.Object,
                null,
                null);

            readApplicationsRepositoryMock.Setup(ra => ra.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ApplicationEntity)null);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
