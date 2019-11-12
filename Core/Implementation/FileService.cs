using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dto;
using Common.Exceptions;
using Core.DtoValidators;
using Core.FileProcessor;

namespace Core.Implementation
{
    public class FileService : IFileService
    {
        private readonly string ROOT_FOLDER_PATH;

        private readonly IFileProcessorFactory _fileProcessorFactory;
        private readonly IConfigurationManager _configurationManager;

        public FileService(IFileProcessorFactory fileProcessorFactory, IConfigurationManager configurationManager)
        {
            _fileProcessorFactory = fileProcessorFactory;
            _configurationManager = configurationManager;

            ROOT_FOLDER_PATH = configurationManager.GetRootFolderPath();
        }

        public Task<List<FileNodeDto>> GetAll()
        {
            return Task.FromResult(BuildFilesTree(ROOT_FOLDER_PATH));
        }

        public async Task<FileDataDto> GetData(FileParseOptionsDto parseOptions)
        {
            FileParseOptionsDtoValidator.ValidateAndThrow(parseOptions);

            CheckRootPath(parseOptions.FullName);
            var fileProcessor = GetFileProcessor(parseOptions.FullName);

            return await fileProcessor.Read(parseOptions);
        }

        public async Task UpdateData(FileDataDto fileData)
        {
            FileDataDtoValidator.ValidateAndThrow(fileData);

            CheckRootPath(fileData.FullName);
            var fileProcessor = GetFileProcessor(fileData.FullName);

            await fileProcessor.Update(fileData);
        }

        private void CheckRootPath(string fileFullName)
        {
            if (!fileFullName.StartsWith(ROOT_FOLDER_PATH, StringComparison.OrdinalIgnoreCase))
                throw new BusinessLogicException(Resources.ErrorMessages.FilesDirectoryPathInvalid);
        }

        private IFileProcessor GetFileProcessor(string fileFullName)
        {
            var fileExtesion = GetLastStringAfterDelimiter(fileFullName, ".");
            return _fileProcessorFactory.Create(fileExtesion);
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
