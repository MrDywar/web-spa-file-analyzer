﻿using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DtoValidators
{
    public abstract class Validator<TVal, TDto> : AbstractValidator<TDto> where TVal : IValidator, new()
    {
        public static void ValidateAndThrow(TDto value)
        {
            if (value == null)
            {
                throw new Common.Exceptions.ValidationException(Resources.ErrorMessages.NullValidationObject);
            }

            var validator = new TVal();
            var result = validator.Validate(value);

            if (!result.IsValid)
            {
                throw new Common.Exceptions.ValidationException(result.ToString());
            }
        }
    }
}
