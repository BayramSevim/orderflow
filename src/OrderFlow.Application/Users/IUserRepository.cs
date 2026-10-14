using OrderFlow.Domain.Users;

namespace OrderFlow.Application.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
        Task AddAsync(User product, CancellationToken cancellationToken);
    }
}