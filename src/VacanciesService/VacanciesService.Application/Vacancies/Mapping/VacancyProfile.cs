using AutoMapper;
using VacanciesService.Application.Vacancies.Commands.AddVacancyCommand;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Mapping
{
    public class VacancyProfile : Profile
    {
        public VacancyProfile()
        {
            CreateMap<AddVacancyCommand, VacancyEntity>();
            CreateMap<VacancyEntity, Vacancy>();
            CreateMap<Domain.Models.Application, ApplicationEntity>().ReverseMap();
        }
    }
}
