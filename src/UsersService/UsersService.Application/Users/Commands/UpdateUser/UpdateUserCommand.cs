using MediatR;

namespace UsersService.Application.Users.Commands.UpdateUser
{
    public sealed record UpdateUserCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string PhoneNumber) : IRequest<Guid>;
}
