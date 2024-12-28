using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UsersService.Application.Companies.Commands.DeleteCompany;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Tests.Unit.Companies
{
    public class DeleteCompanyCommandTests
    {
        private readonly Mock<ILogger<DeleteCompanyCommandHandler>> _logger;
        public DeleteCompanyCommandTests()
        {
            _logger = new Mock<ILogger<DeleteCompanyCommandHandler>>();
        }

        [Fact]
        public async Task ShouldDelete_WhenCompanyExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new DeleteCompanyCommandHandler(_logger.Object, unitOfWorkMock.Object);

            var id = Guid.NewGuid();
            var command = new DeleteCompanyCommand(id);
            var companyEntity = new CompanyEntity { Id = id };

            unitOfWorkMock.Setup(u => u.CompaniesRepository.GetWithIncludesAsync(id, CancellationToken.None)).ReturnsAsync(companyEntity);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            var idAct = await handler.Handle(command, CancellationToken.None);

            // Assert
            idAct.Should().Be(id);

            unitOfWorkMock.Verify(
                u => u.CompaniesRepository.GetWithIncludesAsync(companyEntity.Id, CancellationToken.None),
                Times.Once,
                "Get method should be called once");

            unitOfWorkMock.Verify(
                u => u.CompaniesRepository.Remove(companyEntity),
                Times.Once,
                "Remove method should be called once");

            unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(CancellationToken.None),
                Times.Once,
                "Save changes should be called once in context");
        }

        [Fact]
        public async Task ShouldThrowEntityNotFoundException_WhenCompanyNotExist()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var handler = new DeleteCompanyCommandHandler(_logger.Object, unitOfWorkMock.Object);

            var id = Guid.NewGuid();
            var command = new DeleteCompanyCommand(id);

            unitOfWorkMock.Setup(u => u.CompaniesRepository.GetAsync(id, CancellationToken.None)).ReturnsAsync((CompanyEntity)null);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
