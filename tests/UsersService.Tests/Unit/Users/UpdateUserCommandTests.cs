using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Users.Commands.UpdateUser;
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

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByIdAsync(command.Id)).ReturnsAsync(userEntity);
            unitOfWorkMock.Setup(u => u.UsersRepository.UpdateAsync(userEntity)).ReturnsAsync(IdentityResult.Success);
            mapperMock.Setup(m => m.Map(command, userEntity)).Returns(userEntity);

            // Act
            var idAct = await handler.Handle(command, CancellationToken.None);

            // Assert
            idAct.Should().Be(command.Id);

            unitOfWorkMock.Verify(
                u => u.UsersRepository.GetByIdAsync(command.Id),
                Times.Once,
                "Get method should be called once");

            unitOfWorkMock.Verify(
                u => u.UsersRepository.UpdateAsync(userEntity),
                Times.Once,
                "Update method should be called once");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenUserNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateUserCommandHandler(
                _loggerMock.Object,
                null,
                unitOfWorkMock.Object);

            var command = GetCommand();
            var userEntity = GetUserEntityFromCommand(command);

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByIdAsync(command.Id)).ReturnsAsync((UserEntity)null);

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
                faker.Random.Guid(),
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Phone.PhoneNumber());
        }
    }
}
