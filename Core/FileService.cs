using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Common.Dto;
using Core.FilePrcossor;

namespace Core
{
    public class FileService : IFileService
    {
        private readonly string rootFolderPath = HostingEnvironment.MapPath("~/App_Data");
        private readonly IFileProcessorFactory _fileProcessorFactory;

        public FileService(IFileProcessorFactory fileProcessorFactory)
        {
            _fileProcessorFactory = fileProcessorFactory;
        }

        public Task<List<FileNodeDto>> GetAll()
        {
            return Task.FromResult(BuildFilesTree(rootFolderPath));
        }

        public async Task<FileDataDto> GetData(FileParseOptionsDto parseOptions)
        {
            CheckRootPath(parseOptions.FullName);
            var fileProcessor = GetFileProcessor(parseOptions.FullName);

            return await fileProcessor.Read(parseOptions);
        }

        public async Task UpdateData(FileDataDto fileData)
        {
            CheckRootPath(fileData.FullName);
            var fileProcessor = GetFileProcessor(fileData.FullName);

            await fileProcessor.Update(fileData);
        }

        private void CheckRootPath(string fileFullName)
        {
            if (!fileFullName.StartsWith(rootFolderPath, StringComparison.OrdinalIgnoreCase))
                throw new Exception();
        }

        private IFileProcessor GetFileProcessor(string fileFullName)
        {
            var fileExtesion = GetLastStringAfterDelimiter(fileFullName, ".");

            if (!_fileProcessorFactory.TryCreate(fileExtesion, out var fileProcessor))
                throw new Exception();

            return fileProcessor;
        }

        private List<FileNodeDto> BuildFilesTree(string root)
        {
            var rootFolder = new FileNodeDto() { FullName = root };

            var dirs = new Stack<FileNodeDto>();
            dirs.Push(rootFolder);

            while (dirs.Count > 0)
            {
                var currentDir = dirs.Pop();

                var subDirs = Directory.GetDirectories(currentDir.FullName);
                foreach (var item in subDirs)
                {
                    var dir = new FileNodeDto()
                    {
                        Name = GetLastStringAfterDelimiter(item, "\\"),
                        FullName = item,
                        IsFolder = true
                    };

                    currentDir.Children.Add(dir);
                    dirs.Push(dir);
                }

                var files = Directory.GetFiles(currentDir.FullName);
                foreach (string item in files)
                {
                    currentDir.Children.Add(new FileNodeDto() 
                    {
                        Name = GetLastStringAfterDelimiter(item, "\\"),
                        FullName = item,
                        IsFolder = false
                    });
                }
            }

            return rootFolder.Children;
        }

        private string GetLastStringAfterDelimiter(string value, string delimiter)
        {
            var pos = value.LastIndexOf(delimiter) + 1;

            return value.Substring(pos, value.Length - pos);
        }
    }
}
