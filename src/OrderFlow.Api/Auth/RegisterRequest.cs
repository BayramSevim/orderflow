using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFlow.Api.Auth;

public record RegisterRequest(string Email, string Password, string Role, Guid? DealerId);
