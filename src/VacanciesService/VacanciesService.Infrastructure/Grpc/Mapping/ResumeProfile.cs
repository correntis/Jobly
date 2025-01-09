using AutoMapper;
using Jobly.Protobufs.Users;
using VacanciesService.Application.VacanciesDetails.Mapping.Converters;
using VacanciesService.Domain.Models;

namespace VacanciesService.Infrastructure.Grpc.Mapping
{
    public class ResumeProfile : Profile
    {
        public ResumeProfile()
        {
            CreateMap<List<string>, List<string>>().ConvertUsing<NullIfEmptyListConverter<string, string>>();

            CreateMap<LanguageMessage, Language>();
            CreateMap<ResumeMessage, Resume>()
                .ForMember(r => r.UserId, m => m.MapFrom(rm => Guid.Parse(rm.UserId)))
                .ForMember(r => r.Skills, m => m.MapFrom(rm => rm.Skills.ToList()))
                .ForMember(r => r.Tags, m => m.MapFrom(rm => rm.Tags.ToList()))
                .ForMember(r => r.Languages, m => m.MapFrom(rm => rm.Languages.ToList()));
        }
    }
}
