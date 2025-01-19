using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Companies.Queries.GetCompanyByUser
{
    internal class GetCompanyByUserQueryHandler : IRequestHandler<GetCompanyByUserQuery, Company>
    {
        private readonly ILogger<GetCompanyByUserQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCompanyByUserQueryHandler(
            ILogger<GetCompanyByUserQueryHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Company> Handle(GetCompanyByUserQuery request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {QueryName} for user with Id {UserId}", request.GetType().Name, request.UserId);

            var companyEntity = await _unitOfWork.CompaniesRepository.GetByUserAsync(request.UserId, cancellationToken)
                ?? throw new EntityNotFoundException($"Company for user with id {request.UserId} not found");

            _logger.LogInformation("Successfully handled {QueryName} for user with Id {UserId}", request.GetType().Name, request.UserId);

            return _mapper.Map<Company>(companyEntity);
        }
    }
}
