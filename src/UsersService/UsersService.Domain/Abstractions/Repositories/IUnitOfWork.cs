namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICompaniesRepository CompaniesRepository { get; }
        IResumesRepository ResumesRepository { get; }
        IUsersRepository UsersRepository { get; }

        Task SaveChangesAsync();
    }
}