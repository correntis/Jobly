using Jobly.Protobufs.Users;
using Jobly.Protobufs.Users.Client;

namespace VacanciesService.Infrastructure.Grpc
{
    public class UsersService
    {
        private readonly UsersGrpcService.UsersGrpcServiceClient _usersService;

        public UsersService(UsersGrpcService.UsersGrpcServiceClient usersService)
        {
            _usersService = usersService;
        }

        public async Task<bool> IsUserExists(Guid id)
        {
            var userExistenceRequest = new UserExistenceRequest() { UserId = id.ToString() };

            var userExistenceResponse = await _usersService.IsUserExistsAsync(userExistenceRequest);

            return userExistenceResponse.Exists;
        }

        public async Task<bool> IsCompanyExists(Guid id)
        {
            var companyExistenceRequest = new CompanyExistenceRequest() { CompanyId = id.ToString() };

            var companyExistenceResponse = await _usersService.IsCompanyExistsAsync(companyExistenceRequest);

            return companyExistenceResponse.Exists;
        }
    }
}
