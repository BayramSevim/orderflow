using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace OrderFlow.Api.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(72);
            RuleFor(x => x.Role).NotEmpty().Must(r => r == "Admin" || r == "Dealer");
        }
    }
}