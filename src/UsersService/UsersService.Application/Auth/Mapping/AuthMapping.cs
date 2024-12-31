using AutoMapper;
using Jobly.Brokers.Models;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Application.Auth.Mapping
{
    public class AuthMapping : Profile
    {
        public AuthMapping()
        {
            CreateMap<UserEntity, RegistrationEvent>()
                .ForMember(registrationEvent => registrationEvent.UserId, mapper => mapper.MapFrom(entity => entity.Id))
                .ForMember(registrationEvent => registrationEvent.UserName, mapper => mapper.MapFrom(entity => entity.UserName));
        }
    }
}
