using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Users;

namespace OrderFlow.Application.Common
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}