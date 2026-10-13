using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message) { }
    }
}