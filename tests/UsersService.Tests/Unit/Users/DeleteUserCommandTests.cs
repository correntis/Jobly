using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Users.Commands.DeleteUserCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Unit.Users
{
    public class DeleteUserCommandTests
    {
        private readonly Mock<ILogger<DeleteUserCommandHandler>> _logger;
        public DeleteUserCommandTests()
        {
            _logger = new Mock<ILogger<DeleteUserCommandHandler>>();
        }

        [Fact]
        public async Task ShouldDelete_WhenUserExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new DeleteUserCommandHandler(_logger.Object, unitOfWorkMock.Object);

            var id = 1;
            var command = new DeleteUserCommand(id);
            var userEntity = new UserEntity { Id = id };

            unitOfWorkMock.Setup(u => u.UsersRepository.GetAsync(id, CancellationToken.None)).ReturnsAsync(userEntity);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var idAct = await handler.Handle(command, CancellationToken.None);

            // Assert
            idAct.Should().Be(id);

            unitOfWorkMock.Verify(
                u => u.UsersRepository.GetAsync(userEntity.Id, CancellationToken.None),
                Times.Once,
                "Get method should be called once");

            unitOfWorkMock.Verify(
                u => u.UsersRepository.Remove(userEntity),
                Times.Once,
                "Remove method should be called once");

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(),
                Times.Once,
                "Save changes should be called once in context");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenUserNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new DeleteUserCommandHandler(_logger.Object, unitOfWorkMock.Object);

            var id = 1;
            var command = new DeleteUserCommand(id);

            unitOfWorkMock.Setup(u => u.UsersRepository.GetAsync(id, CancellationToken.None)).ReturnsAsync((UserEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
