using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FilePrcossor
{
    public interface IFileProcessor
    {
        Task<FileDataDto> Read(FileParseOptionsDto parseOptions);
        Task Update(FileDataDto fileData);
    }
}
