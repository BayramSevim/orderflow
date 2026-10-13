using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Application.Dealers
{
    public class DealerService
    {
        private readonly IDealerRepository _dealerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DealerService(IDealerRepository dealerRepository, IUnitOfWork unitOfWork)
        {
            _dealerRepository = dealerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(string code, string name, decimal creditLimit, CancellationToken cancellationToken)
        {
            if (await _dealerRepository.CodeExistsAsync(code, cancellationToken))
                throw new DomainException("Dealer code already exists");

            var dealer = new Dealer(code, name, creditLimit);

            await _dealerRepository.AddAsync(dealer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return dealer.Id;
        }

        public async Task<DealerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var dealer = await _dealerRepository.GetByIdAsync(id, cancellationToken);

            if (dealer is null)
                return null;

            return new DealerResponse(dealer.Id, dealer.Code, dealer.Name, dealer.CreditLimit, dealer.IsActive);
        }

        public async Task<List<DealerResponse>> GetDealersAsync(CancellationToken cancellationToken)
        {
            var dealers = await _dealerRepository.GetDealersAsync(cancellationToken);

            return dealers
                    .Select(d => new DealerResponse(d.Id, d.Code, d.Name, d.CreditLimit, d.IsActive))
                    .ToList();
        }
    }
}