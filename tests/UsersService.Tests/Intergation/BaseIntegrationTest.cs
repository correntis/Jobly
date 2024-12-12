using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace UsersService.Tests.Intergation
{
    public abstract class BaseIntegrationTest : IClassFixture<IntergationTestWebAppFactory>
    {
        protected readonly ISender Sender;

        private readonly IServiceScope _scope;

        protected BaseIntegrationTest(IntergationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        }
    }
}
