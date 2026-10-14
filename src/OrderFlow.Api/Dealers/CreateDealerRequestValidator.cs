using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace OrderFlow.Api.Dealers
{
    public class CreateDealerRequestValidator : AbstractValidator<CreateDealerRequest>
    {
        public CreateDealerRequestValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
        }
    }
}