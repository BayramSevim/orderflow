using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Dealers;
using OrderFlow.Domain.Dealers;

namespace OrderFlow.Application.Tests.Fakes
{
    public class FakeDealerRepository : IDealerRepository
    {
        public List<Dealer> Dealers { get; } = new();
        public Task AddAsync(Dealer dealer, CancellationToken cancellationToken)
        {
            Dealers.Add(dealer);
            return Task.CompletedTask;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
        {
            return Task.FromResult(Dealers.Any(d => d.Code == code));
        }

        public Task<Dealer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Dealers.FirstOrDefault(d => d.Id == id));
        }

        public Task<List<Dealer>> GetDealersAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Dealers);
        }
    }
}