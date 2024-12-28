using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Companies.Queries.GetCompany
{
    public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, Company>
    {
        private readonly ILogger<GetCompanyQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCompanyQueryHandler(
            ILogger<GetCompanyQueryHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Company> Handle(GetCompanyQuery request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {QueryName} for company with ID {CompanyId}", request.GetType().Name, request.Id);

            var companyEntity = await _unitOfWork.CompaniesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Company with id {request.Id} not found");

            _logger.LogInformation("Successfully handled {QueryName} company with ID {CompanyId}", request.GetType().Name, request.Id);

            return _mapper.Map<Company>(companyEntity);
        }
    }
}
