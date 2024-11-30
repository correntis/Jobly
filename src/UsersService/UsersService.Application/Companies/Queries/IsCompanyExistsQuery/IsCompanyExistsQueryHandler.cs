using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;

namespace UsersService.Application.Companies.Queries.IsCompanyExistsQuery
{
    public class IsCompanyExistsQueryHandler : IRequestHandler<IsCompanyExistsQuery, bool>
    {
        private readonly ILogger<IsCompanyExistsQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public IsCompanyExistsQueryHandler(
            ILogger<IsCompanyExistsQueryHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(IsCompanyExistsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for company with ID {CompanyId}",
                request.GetType().Name,
                request.Id);

            var isCompanyExists = await _unitOfWork.CompaniesRepository.IsExists(request.Id);

            _logger.LogInformation(
                "Successfully handled {CommandName} for company with ID {CompanyId}",
                request.GetType().Name,
                request.Id);

            return isCompanyExists;
        }
    }
}
