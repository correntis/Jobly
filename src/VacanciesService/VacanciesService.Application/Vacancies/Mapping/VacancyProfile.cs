using AutoMapper;
using VacanciesService.Application.Vacancies.Commands.AddVacancy;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Filters.VacancyDetails;
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

            CreateMap<VacancyInteractionEntity, VacancyInteraction>();
            CreateMap<Language, LanguageFilter>();
            CreateMap<Resume, VacancyDetailsFilter>()
                .ForMember(f => f.Languages, m => m.MapFrom(r => 1 == 1 ? null : r.Skills));
        }
    }
}
