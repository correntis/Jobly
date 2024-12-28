using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.Interactions.Commands.AddInteraction;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Tests.Unit.Interactions
{
    public class AddInteractionCommandTests
    {
        private readonly Mock<ILogger<AddInteractionCommandHandler>> _loggerMock;

        public AddInteractionCommandTests()
        {
            _loggerMock = new Mock<ILogger<AddInteractionCommandHandler>>();
        }

        [Fact]
        public async Task ShouldAdd_WhenValid()
        {
            // Arrange
            var command = new AddInteractionCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

            var mapperMock = new Mock<IMapper>();
            var readVacanciesRepositoryMock = new Mock<IReadVacanciesRepository>();
            var writeInteractionsRepositoryMock = new Mock<IWriteInteractionsRepository>();
            var usersServiceMock = new Mock<IUsersService>();

            var handler = new AddInteractionCommandHandler(
                _loggerMock.Object,
                writeInteractionsRepositoryMock.Object,
                readVacanciesRepositoryMock.Object,
                usersServiceMock.Object,
                mapperMock.Object);

            usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            readVacanciesRepositoryMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacancyEntity());
            mapperMock.Setup(m => m.Map<VacancyInteractionEntity>(It.IsAny<AddInteractionCommand>()))
                .Returns(new VacancyInteractionEntity());

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            writeInteractionsRepositoryMock.Verify(
                wi => wi.AddAsync(It.IsAny<VacancyInteractionEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);

            writeInteractionsRepositoryMock.Verify(
                wi => wi.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenUserNotExist()
        {
            // Arrange
            var command = new AddInteractionCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

            var readVacanciesRepositoryMock = new Mock<IReadVacanciesRepository>();

            var handler = new AddInteractionCommandHandler(
                _loggerMock.Object,
                null,
                readVacanciesRepositoryMock.Object,
                null,
                null);

            readVacanciesRepositoryMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((VacancyEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenVacancyNotExist()
        {
            // Arrange
            var command = new AddInteractionCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

            var readVacanciesRepositoryMock = new Mock<IReadVacanciesRepository>();
            var usersServiceMock = new Mock<IUsersService>();

            var handler = new AddInteractionCommandHandler(
                _loggerMock.Object,
                null,
                readVacanciesRepositoryMock.Object,
                usersServiceMock.Object,
                null);

            usersServiceMock.Setup(us => us.IsUserExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));
            readVacanciesRepositoryMock.Setup(rv => rv.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacancyEntity());

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
