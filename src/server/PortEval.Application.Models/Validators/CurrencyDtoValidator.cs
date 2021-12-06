﻿using FluentValidation;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Models.Validators
{
    public class CurrencyDtoValidator : AbstractValidator<CurrencyDto>
    {
        public CurrencyDtoValidator()
        {
            RuleFor(c => c.Code)
                .NotNull()
                .Length(3);
            RuleFor(c => c.IsDefault)
                .NotNull();
            RuleFor(c => c.Name)
                .MaximumLength(64);
            RuleFor(c => c.Symbol)
                .MaximumLength(4);
        }
    }
}
