using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Exceptions
{
    public class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message) { }
    }
}