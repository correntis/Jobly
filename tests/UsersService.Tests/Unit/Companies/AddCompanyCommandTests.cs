using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Companies.Commands.AddCompanyCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Unit.Companies
{
    public class AddCompanyCommandTests
    {
        private readonly Mock<ILogger<AddCompanyCommandHandler>> _loggerMock;

        public AddCompanyCommandTests()
        {
            _loggerMock = new Mock<ILogger<AddCompanyCommandHandler>>();
        }

        [Fact]
        public async Task ShouldAdd_WhenCompanyValid()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var imagesServiceMock = new Mock<IImagesService>();
            var mapperMock = new Mock<IMapper>();

            var handler = new AddCompanyCommandHandler(_loggerMock.Object, unitOfWorkMock.Object, mapperMock.Object, imagesServiceMock.Object);

            var command = GetCommand();
            var companyEntity = GetCompanyEntityFromCommand(command);
            var userEntity = new UserEntity();
            var logoPath = "//path";

            unitOfWorkMock.Setup(u => u.UsersRepository.GetAsync(command.UserId, CancellationToken.None)).ReturnsAsync(userEntity);
            unitOfWorkMock.Setup(u => u.CompaniesRepository.AddAsync(companyEntity, CancellationToken.None)).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);
            mapperMock.Setup(m => m.Map<CompanyEntity>(command)).Returns(companyEntity);
            imagesServiceMock.Setup(i => i.SaveAsync(command.Image, CancellationToken.None)).ReturnsAsync(logoPath);

            // Act
            var idAct = await handler.Handle(command, CancellationToken.None);

            // Arrange
            idAct.Should().Be(companyEntity.Id);
            companyEntity.LogoPath.Should().Be(logoPath);
            companyEntity.User.Should().Be(userEntity);

            unitOfWorkMock.Verify(
                u => u.UsersRepository.GetAsync(command.UserId, CancellationToken.None),
                Times.Once,
                "Get method should be called once in users repository");

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(CancellationToken.None),
                Times.Once,
                "Save changes should be called once in context");

            unitOfWorkMock.Verify(
                u => u.CompaniesRepository.AddAsync(companyEntity, CancellationToken.None),
                Times.Once,
                "Add method should be called once in companies repository");

            imagesServiceMock.Verify(
                i => i.SaveAsync(command.Image, CancellationToken.None),
                Times.Once,
                "Save image method should be called once");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenUserForCompanyNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new AddCompanyCommandHandler(_loggerMock.Object, unitOfWorkMock.Object, null, null);
            var command = GetCommand();

            unitOfWorkMock.Setup(u => u.UsersRepository.GetAsync(command.UserId, CancellationToken.None)).ReturnsAsync((UserEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Arrange
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        public CompanyEntity GetCompanyEntityFromCommand(AddCompanyCommand command)
        {
            return new CompanyEntity()
            {
                Id = 1,
                UserId = command.UserId,
                Name = command.Name,
                Address = command.Address,
                City = command.City,
                Email = command.Email,
                Type = command.Type,
            };
        }

        public AddCompanyCommand GetCommand()
        {
            var faker = new Faker();

            return new AddCompanyCommand(
                faker.Random.Int(0),
                faker.Company.CompanyName(),
                faker.Address.City(),
                faker.Address.SecondaryAddress(),
                faker.Internet.Email(),
                faker.Company.CompanySuffix(),
                null);
        }
    }
}
