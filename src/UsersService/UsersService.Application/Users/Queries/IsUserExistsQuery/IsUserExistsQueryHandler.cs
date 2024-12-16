using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;

namespace UsersService.Application.Users.Queries.IsUserExistsQuery
{
    public class IsUserExistsQueryHandler : IRequestHandler<IsUserExistsQuery, bool>
    {
        private readonly ILogger<IsUserExistsQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public IsUserExistsQueryHandler(
            ILogger<IsUserExistsQueryHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(IsUserExistsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for user with ID {UserId}",
                request.GetType().Name,
                request.Id);

            var isUserExists = await _unitOfWork.UsersRepository.Exists(request.Id);

            _logger.LogInformation(
                "Successfully handled {CommandName} for user with ID {UserId}",
                request.GetType().Name,
                request.Id);

            return isUserExists;
        }
    }
}
