using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VacanciesService.Domain.Abstractions.Services;

namespace VacanciesService.Tests.Integration
{
    public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        protected readonly ISender Sender;
        protected readonly Mock<IUsersService> UsersServiceMock;

        private readonly IServiceScope _scope;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetService<ISender>();

            UsersServiceMock = factory.UsersServiceMock;
        }
    }
}
