using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}