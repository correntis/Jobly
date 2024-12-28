using Grpc.Core;
using Jobly.Protobufs.Users;
using Jobly.Protobufs.Users.Server;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Application.Companies.Queries.IsCompanyExists;
using UsersService.Application.Resumes.Queries.GetResume;
using UsersService.Application.Users.Queries.IsUserExists;
using UsersService.Domain.Models;

namespace UsersService.Presentation.Controllers.Grpc
{
    public class UsersServiceController : UsersGrpcService.UsersGrpcServiceBase
    {
        private readonly ILogger<UsersServiceController> _logger;
        private readonly ISender _sender;

        public UsersServiceController(
            ILogger<UsersServiceController> logger,
            ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public override async Task<CompanyExistenceResponse> IsCompanyExists(CompanyExistenceRequest request, ServerCallContext context)
        {
            _logger.LogInformation("[GRPC] Start proccessing gRPC request {RequestName}", request.GetType().Name);

            if (!Guid.TryParse(request.CompanyId, out Guid companyId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid GUID {request.CompanyId}"));
            }

            var isExists = await _sender.Send(new IsCompanyExistsQuery(companyId));

            _logger.LogInformation("[GRPC] Successfully proccessed gRPC request {RequestName}", request.GetType().Name);

            return new CompanyExistenceResponse { Exists = isExists };
        }

        public async override Task<UserExistenceResponse> IsUserExists(UserExistenceRequest request, ServerCallContext context)
        {
            _logger.LogInformation("[GRPC] Start proccessing gRPC request {RequestName}", request.GetType().Name);

            if (!Guid.TryParse(request.UserId, out Guid userId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid GUID {request.UserId}"));
            }

            var isExists = await _sender.Send(new IsUserExistsQuery(userId));

            _logger.LogInformation("[GRPC] Successfully proccessed gRPC request {RequestName}", request.GetType().Name);

            return new UserExistenceResponse { Exists = isExists };
        }

        public override async Task<GetResumeResponse> GetResume(GetResumeRequest request, ServerCallContext context)
        {
            _logger.LogInformation(
                "[GRPC] Start proccessing gRPC request {RequestName} for resume with ID {ResumeId}",
                request.GetType().Name,
                request.ResumeId);

            var resume = await _sender.Send(new GetResumeQuery(request.ResumeId));

            if (resume is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Resume with ID {request.ResumeId} not found"));
            }

            _logger.LogInformation(
                "[GRPC] Successfully proccessed gRPC request {RequestName} for resume with ID {ResumeId}",
                request.GetType().Name,
                request.ResumeId);

            return GetResumeResponse(resume);
        }

        private GetResumeResponse GetResumeResponse(Resume resume)
        {
            var response = new GetResumeResponse()
            {
                Resume = new ResumeMessage()
                {
                    Id = resume.Id,
                    UserId = resume.UserId.ToString(),
                },
            };

            response.Resume.Skills.AddRange(resume.Skills);
            response.Resume.Tags.AddRange(resume.Tags);
            response.Resume.Languages.AddRange(
                resume.Languages.Select(l =>
                    new LanguageMessage()
                    {
                        Name = l.Name,
                        Level = l.Level,
                    }));

            return response;
        }
    }
}
