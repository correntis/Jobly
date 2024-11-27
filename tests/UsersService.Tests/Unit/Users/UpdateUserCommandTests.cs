using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Users.Commands.UpdateUserCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Unit.Users
{
    public class UpdateUserCommandTests
    {
        private readonly Mock<ILogger<UpdateUserCommandHandler>> _loggerMock;

        public UpdateUserCommandTests()
        {
            _loggerMock = new Mock<ILogger<UpdateUserCommandHandler>>();
        }

        [Fact]
        public async Task ShouldUpdate_WhenUserExists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();

            var handler = new UpdateUserCommandHandler(
                _loggerMock.Object,
                mapperMock.Object,
                unitOfWorkMock.Object);

            var command = GetCommand();
            var userEntity = GetUserEntityFromCommand(command);

            unitOfWorkMock.Setup(u => u.UsersRepository.GetAsync(command.Id, CancellationToken.None)).ReturnsAsync(userEntity);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            var idAct = await handler.Handle(command, CancellationToken.None);

            // Assert
            idAct.Should().Be(command.Id);

            unitOfWorkMock.Verify(
                u => u.UsersRepository.GetAsync(command.Id, CancellationToken.None),
                Times.Once,
                "Get method should be called once");

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(CancellationToken.None),
                Times.Once,
                "Save changes should be called once in context");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenUserNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();

            var handler = new UpdateUserCommandHandler(
                _loggerMock.Object,
                mapperMock.Object,
                unitOfWorkMock.Object);

            var command = GetCommand();
            var userEntity = GetUserEntityFromCommand(command);

            unitOfWorkMock.Setup(u => u.UsersRepository.GetAsync(command.Id, CancellationToken.None)).ReturnsAsync((UserEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        public UserEntity GetUserEntityFromCommand(UpdateUserCommand command)
        {
            return new UserEntity
            {
                Id = command.Id,
            };
        }

        public UpdateUserCommand GetCommand()
        {
            var faker = new Faker();

            return new UpdateUserCommand(
                faker.Random.Int(0),
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Phone.PhoneNumber());
        }
    }
}
