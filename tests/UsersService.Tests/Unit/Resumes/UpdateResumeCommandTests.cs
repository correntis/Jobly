using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Resumes.Commands.UpdateResumeCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Unit.Resumes
{
    public class UpdateResumeCommandTests
    {
        private readonly Mock<ILogger<UpdateResumeCommandHandler>> _loggerMock;

        public UpdateResumeCommandTests()
        {
            _loggerMock = new Mock<ILogger<UpdateResumeCommandHandler>>();
        }

        [Fact]
        public async Task ShouldUpdateAndReturnId_WhenResumeExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            var handler = new UpdateResumeCommandHandler(_loggerMock.Object, unitOfWorkMock.Object, mapperMock.Object);

            var command = GetCommand();
            var resumeEntity = GetResumeFromCommand(command);

            unitOfWorkMock.Setup(u => u.ResumesRepository.GetAsync(command.Id, CancellationToken.None)).ReturnsAsync(resumeEntity);
            unitOfWorkMock.Setup(u => u.ResumesRepository.UpdateAsync(resumeEntity, CancellationToken.None)).Returns(Task.CompletedTask);
            mapperMock.Setup(m => m.Map<ResumeEntity>(command)).Returns(resumeEntity);

            // Act
            var idAct = await handler.Handle(command);

            // Assert
            idAct.Should().Be(resumeEntity.Id);

            unitOfWorkMock.Verify(
                u => u.ResumesRepository.GetAsync(command.Id, CancellationToken.None),
                Times.Once,
                "Get method should be called once in resumes repository");

            unitOfWorkMock.Verify(
                u => u.ResumesRepository.UpdateAsync(resumeEntity, CancellationToken.None),
                Times.Once,
                "Update method should be called once in resumes repository");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenResumeNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new UpdateResumeCommandHandler(_loggerMock.Object, unitOfWorkMock.Object, null);

            var command = GetCommand();

            unitOfWorkMock.Setup(u => u.ResumesRepository.GetAsync(command.Id, CancellationToken.None)).ReturnsAsync((ResumeEntity)null);

            // Act
            var act = async () => await handler.Handle(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        public ResumeEntity GetResumeFromCommand(UpdateResumeCommand command)
        {
            return new ResumeEntity()
            {
                Id = command.Id,
                UserId = command.UserId,
                Summary = command.Summary,
                Title = command.Title,
                Skills = command.Skills,
                Tags = command.Tags,
            };
        }

        public UpdateResumeCommand GetCommand()
        {
            var faker = new Faker();

            var skills = Enumerable.Repeat(faker.Random.String(20), faker.Random.Number(20)).ToList();
            var tags = Enumerable.Repeat(faker.Random.String(20), faker.Random.Number(20)).ToList();

            return new UpdateResumeCommand(
                Guid.NewGuid().ToString(),
                faker.Random.Guid(),
                faker.Random.String(20),
                faker.Random.String(20),
                skills,
                tags);
        }
    }
}
