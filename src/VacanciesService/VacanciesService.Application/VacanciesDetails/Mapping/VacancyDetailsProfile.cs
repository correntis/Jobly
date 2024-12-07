using AutoMapper;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetailsCommand;
using VacanciesService.Application.VacanciesDetails.Mapping.Converters;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.VacanciesDetails.Mapping
{
    public class VacancyDetailsProfile : Profile
    {
        public VacancyDetailsProfile()
        {
            CreateMap<List<string>, List<string>>().ConvertUsing<NullIfEmptyListConverter<string, string>>();

            CreateMap<AddVacancyDetailsCommand, VacancyDetailsEntity>();
            CreateMap<VacancyDetailsEntity, VacancyDetails>();
            CreateMap<Salary, SalaryEntity>().ReverseMap();
            CreateMap<OriginalSalary, OriginalSalaryEntity>().ReverseMap();
            CreateMap<ExperienceLevel, ExperienceLevelEntity>().ReverseMap();
            CreateMap<Language, LanguageEntity>().ReverseMap();

            CreateMap<List<Language>, List<LanguageFilter>>()
                .ConvertUsing(src => src == null || src.Count == 0 ? null : src.Select(l => new LanguageFilter
                {
                    Name = l.Name,
                    Level = l.Level,
                }).ToList());

            CreateMap<Resume, VacancyDetailsFilter>()
                .ForMember(f => f.Languages, m => m.MapFrom(r => r.Languages));
        }
    }
}
