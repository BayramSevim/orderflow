using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFlow.Application.Common
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        string Role { get; }
        Guid? DealerId { get; }
        bool IsAdmin { get; }
    }
}