using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Queries.GetBestResumesForVacancy
{
    public class GetBestResumesForVacancyQueryHandler : IRequestHandler<GetBestResumesForVacancyQuery, List<Resume>>
    {
        private readonly ILogger<GetBestResumesForVacancyQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBestResumesForVacancyQueryHandler(
            ILogger<GetBestResumesForVacancyQueryHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<Resume>> Handle(GetBestResumesForVacancyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {CommandName}", request.GetType().Name);

            var languagesEntities = _mapper.Map<List<LanguageEntity>>(request.Languages);

            var resumes = await _unitOfWork.ResumesRepository.FilterForVacancy(request.Skills, request.Tags, languagesEntities);

            _logger.LogInformation("Successfully handled {CommandName}", request.GetType().Name);

            return _mapper.Map<List<Resume>>(resumes);
        }
    }
}
