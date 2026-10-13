using OrderFlow.Domain.Dealers;

namespace OrderFlow.Application.Dealers
{
    public interface IDealerRepository
    {
        Task<Dealer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);
        Task AddAsync(Dealer dealer, CancellationToken cancellationToken);
        Task<List<Dealer>> GetDealersAsync(CancellationToken cancellationToken);
    }
}