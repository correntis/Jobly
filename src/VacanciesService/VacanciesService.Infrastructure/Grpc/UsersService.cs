using AutoMapper;
using Jobly.Protobufs.Users;
using Jobly.Protobufs.Users.Client;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Models;

namespace VacanciesService.Infrastructure.Grpc
{
    public class UsersService : IUsersService
    {
        private readonly UsersGrpcService.UsersGrpcServiceClient _usersGrpcService;
        private readonly IMapper _mapper;

        public UsersService(
            UsersGrpcService.UsersGrpcServiceClient usersGrpcService,
            IMapper mapper)
        {
            _usersGrpcService = usersGrpcService;
            _mapper = mapper;
        }

        public async Task<bool> IsUserExistsAsync(Guid id, CancellationToken token = default)
        {
            var userExistenceRequest = new UserExistenceRequest() { UserId = id.ToString() };

            var userExistenceResponse =
                await _usersGrpcService.IsUserExistsAsync(userExistenceRequest, cancellationToken: token);

            return userExistenceResponse.Exists;
        }

        public async Task<bool> IsCompanyExistsAsync(Guid id, CancellationToken token = default)
        {
            var companyExistenceRequest = new CompanyExistenceRequest() { CompanyId = id.ToString() };

            var companyExistenceResponse =
                await _usersGrpcService.IsCompanyExistsAsync(companyExistenceRequest, cancellationToken: token);

            return companyExistenceResponse.Exists;
        }

        public async Task<Resume> GetResumeAsync(string resumeId, CancellationToken token = default)
        {
            var getResumeRequest = new GetResumeRequest() { ResumeId = resumeId };

            var getResumeResponse =
                await _usersGrpcService.GetResumeAsync(getResumeRequest, cancellationToken: token);

            return _mapper.Map<Resume>(getResumeResponse.Resume);
        }
    }
}
