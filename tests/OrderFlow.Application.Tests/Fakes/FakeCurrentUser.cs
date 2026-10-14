using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Common;
using OrderFlow.Application.Users;
using OrderFlow.Domain.Users;

namespace OrderFlow.Application.Tests.Fakes
{
    public class FakeCurrentUser : ICurrentUser
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = "Admin";
        public Guid? DealerId { get; set; }
        public bool IsAdmin => Role == "Admin";
    }
}