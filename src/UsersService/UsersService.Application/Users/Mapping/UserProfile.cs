using AutoMapper;
using UsersService.Application.Auth.Commands.RegisterUserCommand;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Models;

namespace UsersService.Application.Users.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserEntity, User>();
            CreateMap<RegisterUserCommand, UserEntity>();
        }
    }
}
