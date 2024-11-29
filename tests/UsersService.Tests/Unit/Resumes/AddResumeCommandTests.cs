using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Resumes.Commands.AddResumeCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Unit.Resumes
{
    public class AddResumeCommandTests
    {
        private readonly Mock<ILogger<AddResumeCommandHandler>> _loggerMock;
        public AddResumeCommandTests()
        {
            _loggerMock = new Mock<ILogger<AddResumeCommandHandler>>();
        }

        [Fact]
        public async Task ShouldSaveAndReturnId_WhenResumeValid()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();

            var handler = new AddResumeCommandHandler(_loggerMock.Object, unitOfWorkMock.Object, mapperMock.Object);

            var command = GetCommand();
            var resumeEntity = GetResumeFromCommand(command);
            var userEntity = new UserEntity() { Id = command.UserId };

            unitOfWorkMock.Setup(u => u.UsersRepository.FindByIdAsync(command.UserId.ToString())).ReturnsAsync(userEntity);
            unitOfWorkMock.Setup(u => u.ResumesRepository.GetByAsync(r => r.UserId, command.UserId, CancellationToken.None))
                .ReturnsAsync((ResumeEntity)null);
            unitOfWorkMock.Setup(u => u.ResumesRepository.AddAsync(resumeEntity, CancellationToken.None)).Returns(Task.CompletedTask);
            mapperMock.Setup(m => m.Map<ResumeEntity>(command)).Returns(resumeEntity);

            // Act
            var idAct = await handler.Handle(command);

            // Assert
            idAct.Should().Be(resumeEntity.Id);

            unitOfWorkMock.Verify(
                u => u.UsersRepository.FindByIdAsync(command.UserId.ToString()),
                Times.Once,
                "Get method should be called once in users repository");

            unitOfWorkMock.Verify(
                u => u.ResumesRepository.AddAsync(resumeEntity, CancellationToken.None),
                Times.Once,
                "Add method should be called once in resumes repository");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenUserForResumeNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new AddResumeCommandHandler(_loggerMock.Object, unitOfWorkMock.Object, null);

            var command = GetCommand();

            unitOfWorkMock.Setup(
                u => u.UsersRepository.FindByIdAsync(command.UserId.ToString())).ReturnsAsync((UserEntity)null);

            // Act
            var act = async () => await handler.Handle(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task ShouldThrowEntityAlreadyExistsException_WhenResumeForUserExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new AddResumeCommandHandler(_loggerMock.Object, unitOfWorkMock.Object, null);

            var command = GetCommand();
            var resumeEntity = new ResumeEntity();
            var userEntity = new UserEntity() { Id = command.UserId };

            unitOfWorkMock.Setup(u => u.UsersRepository.FindByIdAsync(command.UserId.ToString())).ReturnsAsync(userEntity);
            unitOfWorkMock.Setup(u => u.ResumesRepository.GetByAsync(r => r.UserId, command.UserId, CancellationToken.None))
                .ReturnsAsync(resumeEntity);

            // Act
            var act = async () => await handler.Handle(command);

            // Assert
            await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        public ResumeEntity GetResumeFromCommand(AddResumeCommand command)
        {
            return new ResumeEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = command.UserId,
                Summary = command.Summary,
                Title = command.Title,
                Skills = command.Skills,
                Tags = command.Tags,
            };
        }

        public AddResumeCommand GetCommand()
        {
            var faker = new Faker();

            var skills = Enumerable.Repeat(faker.Random.String(20), faker.Random.Number(20)).ToList();
            var tags = Enumerable.Repeat(faker.Random.String(20), faker.Random.Number(20)).ToList();

            return new AddResumeCommand(
                faker.Random.Guid(),
                faker.Random.String(20),
                faker.Random.String(20),
                skills,
                tags);
        }
    }
}
