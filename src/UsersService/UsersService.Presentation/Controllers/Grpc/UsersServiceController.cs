using Grpc.Core;
using Jobly.Protobufs.Users;
using Jobly.Protobufs.Users.Server;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Application.Companies.Queries.IsCompanyExistsQuery;
using UsersService.Application.Users.Queries.IsUserExistsQuery;

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
                throw new ArgumentException($"Invalid GUID {request.CompanyId}");
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
                throw new ArgumentException($"Invalid GUID {request.UserId}");
            }

            var isExists = await _sender.Send(new IsUserExistsQuery(userId));

            _logger.LogInformation("[GRPC] Successfully proccessed gRPC request {RequestName}", request.GetType().Name);

            return new UserExistenceResponse { Exists = isExists };
        }
    }
}
