using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VacanciesService.Application.Vacancies.Commands.AddVacancyCommand;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Infrastructure.SQL;

namespace VacanciesService.Tests.Integration.Vacancies
{
    public class AddVacancyCommandTests : BaseIntegrationTest
    {
        private readonly IntegrationTestWebAppFactory _factory;

        private readonly Mock<IUsersService> _mockUsersService;

        public AddVacancyCommandTests(IntegrationTestWebAppFactory factory)
            : base(factory)
        {
            _factory = factory;

            _mockUsersService = factory.UsersServiceMock;
        }

        [Fact]
        public async Task ShouldAdd_WhenValid()
        {
            // Arrange
            var command = GetCommand();

            _mockUsersService.Setup(us => us.IsCompanyExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var idAct = await Sender.Send(command);

            // Assert
            (await CheckIfVacancyExistAsync(idAct)).Should().BeTrue();
        }

        [Fact]
        public async Task ShouldThrowEntityNotFound_WhenCompanyNotExist()
        {
            // Arrange
            var command = GetCommand();

            _mockUsersService.Setup(m => m.IsCompanyExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var act = async () => await Sender.Send(command);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        public AddVacancyCommand GetCommand()
        {
            var faker = new Faker("ru");

            return new AddVacancyCommand(
                faker.Name.JobTitle(),
                faker.Name.JobType(),
                Guid.NewGuid(),
                DateTime.UtcNow.AddMonths(5));
        }

        private async Task<bool> CheckIfVacancyExistAsync(Guid id)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VacanciesWriteContext>();

            var entity = await context.Vacancies.Where(v => v.Id == id).FirstOrDefaultAsync();

            return entity is not null;
        }
    }
}
