using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Models.Validators
{
    public class DateRangeParamsValidator : AbstractValidator<DateRangeParams>
    {
        public DateRangeParamsValidator()
        {
            RuleFor(dr => dr.From)
                .LessThan(dr => dr.To);
        }
    }
}
