using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OrderFlow.Application.Common;

namespace OrderFlow.Api.Auth
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

        private ClaimsPrincipal User => _accessor.HttpContext!.User;

        public Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        public string Role => User.FindFirstValue(ClaimTypes.Role)!;

        public Guid? DealerId
        {
            get
            {
                var value = User.FindFirstValue("dealerId");
                return value is null ? null : Guid.Parse(value);
            }
        }

        public bool IsAdmin => Role == "Admin";
    }
}