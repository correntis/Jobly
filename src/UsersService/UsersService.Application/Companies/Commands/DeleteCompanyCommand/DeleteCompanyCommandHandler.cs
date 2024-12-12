using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Companies.Commands.DeleteCompanyCommand
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Guid>
    {
        private readonly ILogger<DeleteCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCompanyCommandHandler(
            ILogger<DeleteCompanyCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for company with ID {CompanyId}", request.GetType().Name, request.Id);

            var companyEntity = await _unitOfWork.CompaniesRepository.GetWithIncludesAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Company with id {request.Id} not found");

            _unitOfWork.CompaniesRepository.Remove(companyEntity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for company with ID {CompanyId}", request.GetType().Name, request.Id);

            return request.Id;
        }
    }
}
