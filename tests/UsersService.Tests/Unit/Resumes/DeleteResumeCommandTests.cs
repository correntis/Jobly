using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Resumes.Commands.DeleteResumeCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Unit.Resumes
{
    public class DeleteResumeCommandTests
    {
        private readonly Mock<ILogger<DeleteResumeCommandHandler>> _loggerMock;

        public DeleteResumeCommandTests()
        {
            _loggerMock = new Mock<ILogger<DeleteResumeCommandHandler>>();
        }

        [Fact]
        public async Task ShouldDeleteAndReturnId_WhenResumeExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new DeleteResumeCommandHandler(_loggerMock.Object, unitOfWorkMock.Object);

            var id = Guid.NewGuid().ToString();
            var command = new DeleteResumeCommand(id);
            var resumeEntity = new ResumeEntity() { Id = id };

            unitOfWorkMock.Setup(u => u.ResumesRepository.GetAsync(command.Id, CancellationToken.None)).ReturnsAsync(resumeEntity);
            unitOfWorkMock.Setup(u => u.ResumesRepository.DeleteAsync(command.Id, CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            var idAct = await handler.Handle(command);

            // Assert
            idAct.Should().Be(id);

            unitOfWorkMock.Verify(
                u => u.ResumesRepository.GetAsync(command.Id, CancellationToken.None),
                Times.Once,
                "Get method should be called once in resumes repository");

            unitOfWorkMock.Verify(
                u => u.ResumesRepository.DeleteAsync(command.Id, CancellationToken.None),
                Times.Once,
                "Delete method should be called once in resumes repository");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenResumeNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new DeleteResumeCommandHandler(_loggerMock.Object, unitOfWorkMock.Object);

            var command = new DeleteResumeCommand(string.Empty);

            unitOfWorkMock.Setup(u => u.ResumesRepository.GetAsync(command.Id, CancellationToken.None)).ReturnsAsync((ResumeEntity)null);

            // Act
            var act = async () => await handler.Handle(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
