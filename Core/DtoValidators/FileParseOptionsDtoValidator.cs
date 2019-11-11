using Common.Dto;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DtoValidators
{
    public class FileParseOptionsDtoValidator : Validator<FileParseOptionsDtoValidator, FileParseOptionsDto>
    {
        public FileParseOptionsDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
        }
    }
}
