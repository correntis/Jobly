using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Entities.SQL;
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

            _mapper.Map(request, companyEntity);

            if (request.Image is not null)
            {
                await UpdateCompanyLogoAsync(companyEntity, request.Image, cancellationToken);
            }

            _unitOfWork.CompaniesRepository.Update(companyEntity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for company with ID {CompanyId}", request.GetType().Name, request.Id);

            return request.Id;
        }

        private async Task UpdateCompanyLogoAsync(CompanyEntity companyEntity, IFormFile newImage, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(companyEntity.LogoPath))
            {
                _imagesService.Delete(companyEntity.LogoPath);
            }

            companyEntity.LogoPath = await _imagesService.SaveAsync(newImage, cancellationToken);
        }
    }
}
