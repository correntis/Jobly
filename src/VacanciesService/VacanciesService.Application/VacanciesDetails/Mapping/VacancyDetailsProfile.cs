using AutoMapper;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetailsCommand;
using VacanciesService.Application.VacanciesDetails.Mapping.Converters;
using VacanciesService.Domain.Entities.NoSQL;
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
        }
    }
}
