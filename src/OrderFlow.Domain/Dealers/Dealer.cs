using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Dealers
{
    public class Dealer
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public decimal CreditLimit { get; private set; }
        public bool IsActive { get; private set; }

        public Dealer(string code, string name, decimal creditLimit)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("Code is required");
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required");
            if (creditLimit < 0)
                throw new DomainException("Credit limit cannot be negative");

            Id = Guid.NewGuid();
            Code = code;
            Name = name;
            CreditLimit = creditLimit;
            IsActive = true;
        }

        public void ChangeCreditLimit(decimal newLimit)
        {
            if (newLimit < 0)
                throw new DomainException("Credit limit cannot be negative");

            CreditLimit = newLimit;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}