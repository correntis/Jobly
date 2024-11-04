using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Companies.Queries.GetCompanyQuery
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

        public async Task<Company> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {query}", request.GetType().Name);

            var companyEntity = await _unitOfWork.CompaniesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Company with id {request.Id} not found");

            _logger.LogInformation("Successfully handled {query}", request.GetType().Name);

            return _mapper.Map<Company>(companyEntity);
        }
    }
}
