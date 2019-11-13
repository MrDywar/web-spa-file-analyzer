using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IFileService
    {
        Task<List<FileNodeDto>> GetAll();
        Task<FileDataDto> GetData(FileParseOptionsDto parseOptions);
        Task UpdateData(FileDataDto fileData);
    }
}
