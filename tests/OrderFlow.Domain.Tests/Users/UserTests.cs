using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Users;

namespace OrderFlow.Domain.Tests.Users
{
    public class UserTests
    {
        [Fact]
        public void Constructor_DealerWithoutDealerId_Throws()
        {
            Assert.Throws<DomainException>(() => new User("test@test.com", "12345678", "Dealer", null));
        }

        [Fact]
        public void Constructor_AdminWithDealerId_Throws()
        {
            Assert.Throws<DomainException>(() => new User("test@test.com", "12345678", "Admin", Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_InvalidRole_Throws()
        {
            Assert.Throws<DomainException>(() => new User("test@test.com", "12345678", "TEST", null));
        }
    }
}