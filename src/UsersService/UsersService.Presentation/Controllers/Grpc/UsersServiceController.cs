using Grpc.Core;
using Jobly.Protobufs.Users;
using Jobly.Protobufs.Users.Server;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Application.Companies.Queries.IsCompanyExists;
using UsersService.Application.Resumes.Queries.GetBestResumesForVacancy;
using UsersService.Application.Resumes.Queries.GetResume;
using UsersService.Application.Users.Queries.GetUser;
using UsersService.Application.Users.Queries.IsUserExists;
using UsersService.Domain.Exceptions;
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

            var isExists = await _sender.Send(new IsCompanyExistsQuery(companyId), context.CancellationToken);

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

            var isExists = await _sender.Send(new IsUserExistsQuery(userId), context.CancellationToken);

            _logger.LogInformation("[GRPC] Successfully proccessed gRPC request {RequestName}", request.GetType().Name);

            return new UserExistenceResponse { Exists = isExists };
        }

        public override async Task<GetResumeResponse> GetResume(GetResumeRequest request, ServerCallContext context)
        {
            _logger.LogInformation(
                "[GRPC] Start proccessing gRPC request {RequestName} for resume with ID {ResumeId}",
                request.GetType().Name,
                request.ResumeId);

            try
            {
                var resume = await _sender.Send(new GetResumeQuery(request.ResumeId), context.CancellationToken);

                _logger.LogInformation(
                    "[GRPC] Successfully proccessed gRPC request {RequestName} for resume with ID {ResumeId}",
                    request.GetType().Name,
                    request.ResumeId);

                return new GetResumeResponse() { Resume = GetResumeMessage(resume) };
            }
            catch (EntityNotFoundException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Resume with ID {request.ResumeId} not found"));
            }
        }

        public override async Task<GetBestResumesResponse> GetBestResumes(GetBestResumesRequest request, ServerCallContext context)
        {
            _logger.LogInformation("[GRPC] Start proccessing gRPC request {RequestName}", request.GetType().Name);

            var command = new GetBestResumesForVacancyQuery(
                request.Skills.ToList(),
                request.Tags.ToList(),
                request.Languages.Select(language => new Language()
                {
                    Name = language.Name,
                    Level = language.Level,
                }).ToList());

            var resumes = await _sender.Send(command, context.CancellationToken);

            var getBestResumesResponse = new GetBestResumesResponse();

            foreach(var resume in resumes)
            {
                getBestResumesResponse.Resumes.Add(GetResumeMessage(resume));
            }

            _logger.LogInformation("[GRPC] Successfully proccessed gRPC request {RequestName}", request.GetType().Name);

            return getBestResumesResponse;
        }

        public override async Task<GetUserNameResponse> GetUserName(GetUserNameRequest request, ServerCallContext context)
        {
            _logger.LogInformation(
                "[GRPC] Start proccessing gRPC request {RequestName} for user with ID {UserId}",
                request.GetType().Name,
                request.UserId);

            if (!Guid.TryParse(request.UserId, out Guid userId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid GUID {request.UserId}"));
            }

            try
            {
                var user = await _sender.Send(new GetUserQuery(userId), context.CancellationToken);

                _logger.LogInformation(
                    "[GRPC] Successfully proccessed gRPC request {RequestName} for user with ID {UserId}",
                    request.GetType().Name,
                    request.UserId);

                return new GetUserNameResponse() { UserName = user.UserName };
            }
            catch (EntityNotFoundException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with id {request.UserId} not found"));
            }
        }

        private ResumeMessage GetResumeMessage(Resume resume)
        {
            var resumeMessage = new ResumeMessage()
            {
                Id = resume.Id,
                UserId = resume.UserId.ToString(),
            };
                
            resumeMessage.Skills.AddRange(resume.Skills);
            resumeMessage.Tags.AddRange(resume.Tags);
            resumeMessage.Languages.AddRange(
                resume.Languages.Select(l =>
                    new LanguageMessage()
                    {
                        Name = l.Name,
                        Level = l.Level,
                    }));

            return resumeMessage;
        }
    }
}
