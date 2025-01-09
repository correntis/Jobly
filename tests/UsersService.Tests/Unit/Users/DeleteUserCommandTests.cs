using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Users.Commands.DeleteUser;
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

            var id = Guid.NewGuid();
            var command = new DeleteUserCommand(id);
            var userEntity = new UserEntity { Id = id };

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByIdAsync(id)).ReturnsAsync(userEntity);
            unitOfWorkMock.Setup(u => u.UsersRepository.DeleteAsync(userEntity)).ReturnsAsync(IdentityResult.Success);

            // Act
            var idAct = await handler.Handle(command, CancellationToken.None);

            // Assert
            idAct.Should().Be(id);

            unitOfWorkMock.Verify(
                u => u.UsersRepository.GetByIdAsync(userEntity.Id),
                Times.Once,
                "Get method should be called once");

            unitOfWorkMock.Verify(
                u => u.UsersRepository.DeleteAsync(userEntity),
                Times.Once,
                "Remove method should be called once");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenUserNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new DeleteUserCommandHandler(_logger.Object, unitOfWorkMock.Object);

            var id = Guid.NewGuid();
            var command = new DeleteUserCommand(id);

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByIdAsync(id)).ReturnsAsync((UserEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
