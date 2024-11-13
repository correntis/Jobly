using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Companies.Commands.UpdateCompanyCommand
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, int>
    {
        private readonly ILogger<UpdateCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImagesService _imagesService;

        public UpdateCompanyCommandHandler(
            ILogger<UpdateCompanyCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImagesService imagesService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesService = imagesService;
        }

        public async Task<int> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for company with ID {CompanyId}", request.GetType().Name, request.Id);

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

            if (request.Image is not null)
            {
                _imagesService.Delete(companyEntity.LogoPath);

                companyEntity.LogoPath = await _imagesService.SaveAsync(request.Image, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully handled {CommandName} for company with ID {CompanyId}", request.GetType().Name, request.Id);

            return request.Id;
        }
    }
}
