using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Constants;

namespace UsersService.Application.Companies.Commands.AddCompany
{
    public class AddCompanyCommandHandler : IRequestHandler<AddCompanyCommand, Guid>
    {
        private readonly ILogger<AddCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImagesService _imagesService;
        private readonly IUnpValidationService _unpValidationService;

        public AddCompanyCommandHandler(
            ILogger<AddCompanyCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImagesService imagesService,
            IUnpValidationService unpValidationService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imagesService = imagesService;
            _unpValidationService = unpValidationService;
        }

        public async Task<Guid> Handle(AddCompanyCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for user with ID {UserId}", request.GetType().Name, request.UserId);

            var isValidUnp = await _unpValidationService.ValidateUnpAsync(request.Unp, cancellationToken);
            if (!isValidUnp)
            {
                throw new ValidationException(new[]
                {
                    new ValidationError("Unp", "Company with this UNP does not exist or is not active in EGR")
                });
            }

            var userEntity = await _unitOfWork.UsersRepository.GetByIdAsync(request.UserId)
                ?? throw new EntityNotFoundException($"User with id {request.UserId} not found");

            var companyEntity = _mapper.Map<CompanyEntity>(request);

            companyEntity.User = userEntity;
            companyEntity.CreatedAt = DateTime.UtcNow;
            companyEntity.LogoPath = await _imagesService.SaveAsync(request.Image, cancellationToken);

            await _unitOfWork.CompaniesRepository.AddAsync(companyEntity, cancellationToken);

            // Устанавливаем флаг полной регистрации после успешного создания компании
            userEntity.IsFullRegistration = true;
            var updateResult = await _unitOfWork.UsersRepository.UpdateAsync(userEntity);
            if(!updateResult.Succeeded)
            {
                _logger.LogWarning(
                    "Failed to update IsFullRegistration flag for user {UserId}: {Errors}",
                    request.UserId,
                    string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully handled {command} for user with ID {UserId} and company with ID {CompanyId}",
                request.GetType().Name,
                request.UserId,
                companyEntity.Id);

            return companyEntity.Id;
        }
    }
}
