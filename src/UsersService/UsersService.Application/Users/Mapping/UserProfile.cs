using AutoMapper;
using Jobly.Brokers.Models;
using UsersService.Application.Auth.Commands.RegisterUser;
using UsersService.Application.Users.Commands.UpdateUser;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Models;

namespace UsersService.Application.Users.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserEntity, User>();
            CreateMap<UserEntity, RegistrationEvent>()
                .ForMember(registrationEvent => registrationEvent.UserId, mapper => mapper.MapFrom(entity => entity.Id))
                .ForMember(registrationEvent => registrationEvent.UserName, mapper => mapper.MapFrom(entity => entity.UserName));

            CreateMap<RegisterUserCommand, UserEntity>()
                .ForMember(u => u.UserName, dest => dest.MapFrom(r => r.Email.Substring(0, r.Email.IndexOf('@'))))
                .ForMember(u => u.IsFullRegistration, dest => dest.MapFrom(r => r.IsFullRegistration));
            CreateMap<UpdateUserCommand, UserEntity>();
            CreateMap<RoleEntity, Role>().ReverseMap();
        }
    }
}
