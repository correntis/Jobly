using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Companies.Commands.AddCompanyCommand
{
    public class AddCompanyCommandHandler : IRequestHandler<AddCompanyCommand, int>
    {
        private readonly ILogger<AddCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImagesService _imagesService;

        public AddCompanyCommandHandler(
            ILogger<AddCompanyCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImagesService imagesService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesService = imagesService;
        }

        public async Task<int> Handle(AddCompanyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {CommandName} for user with ID {UserId}", request.GetType().Name, request.UserId);

            var userEntity = await _unitOfWork.UsersRepository.GetAsync(request.UserId, cancellationToken)
                ?? throw new EntityNotFoundException($"User with id {request.UserId} not found");

            var companyEntity = _mapper.Map<CompanyEntity>(request);

            companyEntity.User = userEntity;
            companyEntity.CreatedAt = DateTime.Now;
            companyEntity.LogoPath = await _imagesService.SaveAsync(request.Image, cancellationToken);

            await _unitOfWork.CompaniesRepository.AddAsync(companyEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully handled {command} for user with ID {UserId} and company with ID {CompanyId}",
                request.GetType().Name,
                request.UserId,
                companyEntity.Id);

            return companyEntity.Id;
        }
    }
}
