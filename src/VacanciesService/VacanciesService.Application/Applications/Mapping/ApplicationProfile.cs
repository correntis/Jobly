using AutoMapper;
using VacanciesService.Application.Applications.Commands.AddApplicationCommand;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Application.Applications.Mapping
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<AddApplicationCommand, ApplicationEntity>();
        }
    }
}
