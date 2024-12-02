using Jobly.Protobufs.Users;
using Jobly.Protobufs.Users.Client;
using VacanciesService.Domain.Abstractions.Services;

namespace VacanciesService.Infrastructure.Grpc
{
    public class UsersService : IUsersService
    {
        private readonly UsersGrpcService.UsersGrpcServiceClient _usersGrpcService;

        public UsersService(UsersGrpcService.UsersGrpcServiceClient usersGrpcService)
        {
            _usersGrpcService = usersGrpcService;
        }

        public async Task<bool> IsUserExistsAsync(Guid id, CancellationToken token = default)
        {
            var userExistenceRequest = new UserExistenceRequest() { UserId = id.ToString() };

            var userExistenceResponse =
                await _usersGrpcService.IsUserExistsAsync(userExistenceRequest);

            return userExistenceResponse.Exists;
        }

        public async Task<bool> IsCompanyExistsAsync(Guid id, CancellationToken token = default)
        {
            var companyExistenceRequest = new CompanyExistenceRequest() { CompanyId = id.ToString() };

            var companyExistenceResponse =
                await _usersGrpcService.IsCompanyExistsAsync(companyExistenceRequest);

            return companyExistenceResponse.Exists;
        }
    }
}
