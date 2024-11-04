using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Companies.Commands.AddCompanyCommand
{
    public class AddCompanyCommandHandler : IRequestHandler<AddCompanyCommand, int>
    {
        private readonly ILogger<AddCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddCompanyCommandHandler(
            ILogger<AddCompanyCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddCompanyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            var userEntity = await _unitOfWork.UsersRepository.GetAsync(request.UserId, cancellationToken)
                ?? throw new EntityNotFoundException($"User with id {request.UserId} not found");

            var companyEntity = _mapper.Map<CompanyEntity>(request);

            companyEntity.User = userEntity;
            companyEntity.CreatedAt = DateTime.Now;

            // TODO Save logo path
            await _unitOfWork.CompaniesRepository.AddAsync(companyEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return companyEntity.Id;
        }
    }
}
