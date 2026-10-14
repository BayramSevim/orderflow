using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Users;
using OrderFlow.Domain.Users;

namespace OrderFlow.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly OrderFlowDbContext _context;

        public UserRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            return _context.Users.AnyAsync(d => d.Email == email, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return _context.Users.FirstOrDefaultAsync(d => d.Email == email, cancellationToken);
        }
    }
}