using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Companies.Commands.UpdateCompanyCommand
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, int>
    {
        private readonly ILogger<UpdateCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCompanyCommandHandler(
            ILogger<UpdateCompanyCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            var companyEntity = await _unitOfWork.CompaniesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Company with id {request.Id} not found");

            companyEntity.Name = request.Name;
            companyEntity.Description = request.Description;
            companyEntity.City = request.City;
            companyEntity.Address = request.Address;
            companyEntity.Email = request.Email;
            companyEntity.Phone = request.Phone;
            companyEntity.WebSite = request.WebSite;
            companyEntity.Type = request.Type;

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return request.Id;
        }
    }
}
