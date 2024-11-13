using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Companies.Commands.UpdateCompanyCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Unit.Companies
{
    public class UpdateCompanyCommandTests
    {
        private readonly Mock<ILogger<UpdateCompanyCommandHandler>> _loggerMock;
        public UpdateCompanyCommandTests()
        {
            _loggerMock = new Mock<ILogger<UpdateCompanyCommandHandler>>();
        }

        [Fact]
        public async Task ShouldUpdateAndReturnId_WhenCompanyExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            var imagesServiceMock = new Mock<IImagesService>();

            var handler = new UpdateCompanyCommandHandler(
                _loggerMock.Object,
                unitOfWorkMock.Object,
                mapperMock.Object,
                imagesServiceMock.Object);

            var command = GetCommand();
            var companyEntity = GetCompanyEntityFromCommand(command);

            unitOfWorkMock.Setup(u => u.CompaniesRepository.GetAsync(command.Id, CancellationToken.None)).ReturnsAsync(companyEntity);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var idAct = await handler.Handle(command, CancellationToken.None);

            // Assert
            idAct.Should().Be(command.Id);

            unitOfWorkMock.Verify(
                u => u.CompaniesRepository.GetAsync(command.Id, CancellationToken.None),
                Times.Once,
                "Get method should be called once");

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(),
                Times.Once,
                "Save changes should be called once in context");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenCompanyNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var handler = new UpdateCompanyCommandHandler(
                _loggerMock.Object,
                unitOfWorkMock.Object,
                null,
                null);

            var command = GetCommand();

            unitOfWorkMock.Setup(u => u.CompaniesRepository.GetAsync(command.Id, CancellationToken.None)).ReturnsAsync((CompanyEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        public CompanyEntity GetCompanyEntityFromCommand(UpdateCompanyCommand command)
        {
            return new CompanyEntity()
            {
                Id = command.Id,
                Name = command.Name,
                Description = command.Description,
                City = command.City,
                Address = command.Address,
                Email = command.Email,
                Type = command.Type,
            };
        }

        public UpdateCompanyCommand GetCommand()
        {
            var faker = new Faker();

            return new UpdateCompanyCommand(
                faker.Random.Int(0),
                faker.Name.FullName(),
                faker.Company.CatchPhrase(),
                faker.Address.City(),
                faker.Address.SecondaryAddress(),
                faker.Internet.Email(),
                faker.Phone.PhoneNumber(),
                faker.Internet.UrlWithPath(),
                faker.Company.CompanySuffix(),
                null);
        }
    }
}
