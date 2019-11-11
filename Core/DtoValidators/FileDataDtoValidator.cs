using Common.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DtoValidators
{
    public class FileDataDtoValidator : Validator<FileDataDtoValidator, FileDataDto>
    {
        public FileDataDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.Headers).NotEmpty();
        }
    }
}
