using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Auth.Commands.LoginUserCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Tests.Unit.Auth
{
    public class LoginUserCommandTests
    {
        private readonly Mock<ILogger<LoginUserCommandHandler>> _loggerMock;

        public LoginUserCommandTests()
        {
            _loggerMock = new Mock<ILogger<LoginUserCommandHandler>>();
        }

        [Fact]
        public async Task ShouldReturnUserAndToken_WhenCredentialsValid()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var authServiceMock = new Mock<IAuthorizationService>();
            var mapperMock = new Mock<IMapper>();

            var handler = new LoginUserCommandHandler(
                _loggerMock.Object,
                authServiceMock.Object,
                unitOfWorkMock.Object,
                mapperMock.Object);

            var command = GetCommand();
            var token = GetToken();
            var (user, userEntity) = GetUsersFromCommand(command);
            var roles = new List<string> { userEntity.Type };

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByEmailAsync(command.Email, CancellationToken.None)).ReturnsAsync(userEntity);
            authServiceMock.Setup(a => a.IssueTokenAsync(userEntity.Id, roles, CancellationToken.None)).ReturnsAsync(token);
            mapperMock.Setup(m => m.Map<User>(userEntity)).Returns(user);

            // Act
            var (userAct, tokenAct) = await handler.Handle(command, CancellationToken.None);

            // Assert
            userAct.Should().NotBeNull("User should be not null");
            tokenAct.Should().NotBeNull("Token should be not null");
            userAct.Id.Should().Be(userEntity.Id);
            tokenAct.AccessToken.Should().Be(token.AccessToken);

            unitOfWorkMock.Verify(
                u => u.UsersRepository.GetByEmailAsync(command.Email, CancellationToken.None),
                Times.Once,
                "Get by email method should be called");

            authServiceMock.Verify(
                a => a.IssueTokenAsync(userEntity.Id, roles, CancellationToken.None),
                Times.Once,
                "Tokens should be issued");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenEmailInvalid()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new LoginUserCommandHandler(
                _loggerMock.Object,
                null,
                unitOfWorkMock.Object,
                null);

            var command = new LoginUserCommand(null, null);

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByEmailAsync(command.Email, CancellationToken.None)).ReturnsAsync((UserEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task ShouldThrowInvalidPasswordException_WhenPasswordInvalid()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new LoginUserCommandHandler(
                _loggerMock.Object,
                null,
                unitOfWorkMock.Object,
                null);

            var command = GetCommand();
            var (user, userEntity) = GetUsersFromCommand(command);

            command = command with { Password = string.Empty };

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByEmailAsync(command.Email, CancellationToken.None))
                .ReturnsAsync(userEntity);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidPasswordException>();
        }

        public Token GetToken()
        {
            return new Token()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
            };
        }

        public (User, UserEntity) GetUsersFromCommand(LoginUserCommand command)
        {
            var userEntity = new UserEntity()
            {
                Id = 1,
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            };

            var user = new User()
            {
                Id = userEntity.Id,
                Email = userEntity.Email,
                CreatedAt = userEntity.CreatedAt,
            };

            return (user, userEntity);
        }

        public LoginUserCommand GetCommand()
        {
            var faker = new Faker();

            return new LoginUserCommand(
                faker.Internet.Email(),
                faker.Internet.Password());
        }
    }
}
