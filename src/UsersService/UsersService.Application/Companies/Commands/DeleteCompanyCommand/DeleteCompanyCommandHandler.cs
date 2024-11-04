using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Companies.Commands.DeleteCompanyCommand
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, int>
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

        public async Task<int> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            var companyEntity = await _unitOfWork.CompaniesRepository.GetWithIncludesAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Company with id {request.Id} not found");

            _unitOfWork.CompaniesRepository.Remove(companyEntity);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return request.Id;
        }
    }
}
