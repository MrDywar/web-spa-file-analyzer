using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Dto;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebServerCore.Controllers
{
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [Route("api/files"), HttpGet]
        public async Task<List<FileNodeDto>> GetAllFiles()
        {
            return await _fileService.GetAll();
        }

        [Route("api/files/parse"), HttpPost]
        public async Task<FileDataDto> GetFileData([FromBody] FileParseOptionsDto parseOptions)
        {
            return await _fileService.GetData(parseOptions);
        }

        [Route("api/files/update"), HttpPost]
        public async Task UpdateFileData([FromBody] FileDataDto fileData)
        {
            await _fileService.UpdateData(fileData);
        }
    }
}