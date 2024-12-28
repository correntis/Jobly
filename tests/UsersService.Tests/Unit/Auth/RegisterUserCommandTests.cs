using AutoMapper;
using Bogus;
using FluentAssertions;
using Jobly.Brokers.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Auth.Commands.RegisterUser;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Tests.Unit.Auth
{
    public class RegisterUserCommandTests
    {
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _logger;
        public RegisterUserCommandTests()
        {
            _logger = new Mock<ILogger<RegisterUserCommandHandler>>();
        }

        [Fact]
        public async Task ShouldSaveAndReturnToken_WhenEmailNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var authServiceMock = new Mock<IAuthorizationService>();
            var mapperMock = new Mock<IMapper>();
            var brokerProducer = new Mock<IBrokerProcuder>();

            var handler = new RegisterUserCommandHandler(
                _logger.Object,
                authServiceMock.Object,
                mapperMock.Object,
                unitOfWorkMock.Object,
                brokerProducer.Object);

            var command = GetCommand();
            var userEntity = GetUserEntityFromCommand(command);
            var token = GetToken();

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByEmailAsync(command.Email)).ReturnsAsync((UserEntity)null);
            unitOfWorkMock.Setup(u => u.UsersRepository.AddAsync(userEntity, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            unitOfWorkMock.Setup(u => u.RolesRepository.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            unitOfWorkMock.Setup(u => u.UsersRepository.AddToRolesAsync(userEntity, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);
            authServiceMock.Setup(a => a.IssueTokenAsync(userEntity.Id, command.RolesNames, CancellationToken.None)).ReturnsAsync(token);
            mapperMock.Setup(m => m.Map<UserEntity>(command)).Returns(userEntity);

            // Act
            var (idAct, tokenAct) = await handler.Handle(command, CancellationToken.None);

            // Assert
            tokenAct.Should().NotBeNull();
            tokenAct.AccessToken.Should().Be(token.AccessToken);

            unitOfWorkMock.Verify(
                u => u.UsersRepository.GetByEmailAsync(command.Email),
                Times.Once,
                "Get by email method should be called once");

            unitOfWorkMock.Verify(
                u => u.UsersRepository.AddAsync(userEntity, It.IsAny<string>()),
                Times.Once,
                "Add method should be called once in repository");

            authServiceMock.Verify(
                a => a.IssueTokenAsync(userEntity.Id, command.RolesNames, CancellationToken.None),
                Times.Once,
                "Issue token method should be called once");
        }

        [Fact]
        public async Task ShouldReturnEntityAlreadyExistsException_WhenEmailExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userManagerMock = new Mock<UserManager<UserEntity>>();

            var handler = new RegisterUserCommandHandler(
                _logger.Object,
                null,
                null,
                unitOfWorkMock.Object,
                null);

            var command = GetCommand();
            var userEntity = GetUserEntityFromCommand(command);
            var roles = BusinessRules.Roles.All;
            var token = GetToken();

            unitOfWorkMock.Setup(u => u.UsersRepository.GetByEmailAsync(command.Email)).ReturnsAsync(userEntity);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        public Token GetToken()
        {
            return new Token()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
            };
        }

        public UserEntity GetUserEntityFromCommand(RegisterUserCommand command)
        {
            var userEntity = new UserEntity()
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
            };

            return userEntity;
        }

        public RegisterUserCommand GetCommand()
        {
            var faker = new Faker();

            return new RegisterUserCommand(
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Internet.Email(),
                faker.Internet.Password(),
                BusinessRules.Roles.All.ToList());
        }
    }
}
