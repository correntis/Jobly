using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VacanciesService.Application.VacanciesDetails.Commands.DeleteVacancyDetails;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Tests.Unit.VacanciesDetails
{
    public class DeleteVacancyDetailsCommandTests
    {
        private readonly Mock<ILogger<DeleteVacancyDetailsCommandHandler>> _loggerMock;
        public DeleteVacancyDetailsCommandTests()
        {
            _loggerMock = new Mock<ILogger<DeleteVacancyDetailsCommandHandler>>();
        }

        [Fact]
        public async Task ShouldDelete_WhenDetailsExist()
        {
            // Arrange
            var detailsRepMock = new Mock<IVacanciesDetailsRepository>();

            var handler = new DeleteVacancyDetailsCommandHandler(
                _loggerMock.Object,
                detailsRepMock.Object);

            detailsRepMock.Setup(dr => dr.GetByAsync(vd => vd.Id, It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(new VacancyDetailsEntity());
            detailsRepMock.Setup(dr => dr.DeleteByAsync(vd => vd.Id, It.IsAny<string>(), CancellationToken.None));

            // Act
            var act = await handler.Handle(
                new DeleteVacancyDetailsCommand(Guid.NewGuid().ToString()), CancellationToken.None);

            // Assert
            detailsRepMock.Verify(
                dr => dr.GetByAsync(vd => vd.Id, It.IsAny<string>(), CancellationToken.None),
                Times.Once);

            detailsRepMock.Verify(
                dr => dr.DeleteByAsync(vd => vd.Id, It.IsAny<string>(), CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenDetailsNotExist()
        {
            // Arrange
            var detailsRepMock = new Mock<IVacanciesDetailsRepository>();

            var handler = new DeleteVacancyDetailsCommandHandler(
                _loggerMock.Object,
                detailsRepMock.Object);

            detailsRepMock.Setup(rv => rv.GetByAsync(vd => vd.Id, It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync((VacancyDetailsEntity)null);

            // Act
            var act = async () => await handler.Handle(
                new DeleteVacancyDetailsCommand(Guid.NewGuid().ToString()), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
