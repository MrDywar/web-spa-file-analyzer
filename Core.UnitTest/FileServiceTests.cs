using Common.Dto;
using Core.FilePrcossor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.UnitTest
{
    [TestClass]
    public class FileServiceTests
    {
        private const string TEST_ROOT_FOLDER_PATH = @"C:\f1\f2";

        [TestMethod]
        [DeploymentItem("TestFiles", "App_Data")]
        public async Task GetAll_DefaultRootFolder_ReturnsFolderTree()
        {
            var expectedTree = new List<FileNodeDto>()
            {
                new FileNodeDto(@"App_Data\A", "A", true)
                {
                    Children = new List<FileNodeDto>()
                    {
                        new FileNodeDto(@"App_Data\A\A1", "A1", true)
                        {
                            Children = new List<FileNodeDto>()
                            {
                                new FileNodeDto(@"App_Data\A\A1\A1.txt", "A1.txt", false)
                            }
                        },
                        new FileNodeDto(@"App_Data\A\A.txt", "A.txt", false)
                    }
                },
                new FileNodeDto(@"App_Data\empty.txt", "empty.txt", false),
                new FileNodeDto(@"App_Data\tab_file.txt", "tab_file.txt", false)
            };

            var fileService = CreateFileService("App_Data");

            var tree = await fileService.GetAll();

            Assert.IsTrue(CompareTrees(tree, expectedTree));
        }

        [TestMethod]
        public async Task GetData_InvalidRootPath_ThrowException()
        {
            var parseOptions = new FileParseOptionsDto()
            {
                FullName = @"C:\f2\text.txt",
            };

            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH);

            Func<Task<FileDataDto>> act = () => fileService.GetData(parseOptions);

            await Assert.ThrowsExceptionAsync<Exception>(act);
        }

        [TestMethod]
        [DynamicData(nameof(GetInvalidExtensionsFileParseOptionsDto), DynamicDataSourceType.Method)]
        public async Task GetData_InvalidExtensions_ThrowException(FileParseOptionsDto value)
        {
            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH);

            Func<Task<FileDataDto>> act = () => fileService.GetData(value);

            await Assert.ThrowsExceptionAsync<Exception>(act);
        }

        [TestMethod]
        public async Task GetData_ValidFile_ReturnsFileData()
        {
            var parseOptions = new FileParseOptionsDto()
            {
                FullName = $"{TEST_ROOT_FOLDER_PATH}\\text.txt"
            };

            var expectedData = new FileDataDto()
            {
                FullName = $"{TEST_ROOT_FOLDER_PATH}\\text.txt",
            };

            var fileProcFactory = new Mock<IFileProcessorFactory>();
            var fileProcessor = new Mock<IFileProcessor>();

            fileProcessor.Setup(x => x.Read(parseOptions)).Returns(Task.FromResult(expectedData));
            IFileProcessor fp = fileProcessor.Object;
            fileProcFactory.Setup(x => x.TryCreate("txt", out fp)).Returns(true);

            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH, fileProcFactory.Object);

            var resultFileData = await fileService.GetData(parseOptions);

            Assert.ReferenceEquals(resultFileData, expectedData);
        }

        [TestMethod]
        public async Task UpdateData_InvalidRootPath_ThrowException()
        {
            var fileData = new FileDataDto() { FullName = @"C:\text.txt" };
            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH);

            Func<Task> act = () => fileService.UpdateData(fileData);

            await Assert.ThrowsExceptionAsync<Exception>(act);
        }

        [TestMethod]
        public async Task UpdateData_NotSupportedFileExtension_ThrowException()
        {
            var fileData = new FileDataDto()
            {
                FullName = $"{TEST_ROOT_FOLDER_PATH}\\text.txgdsgdsgdst",
            };

            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH);

            Func<Task> act = () => fileService.UpdateData(fileData);

            await Assert.ThrowsExceptionAsync<Exception>(act);
        }

        [TestMethod]
        public async Task UpdateData_ValidFile_ReturnsTask()
        {
            var fileData = new FileDataDto()
            {
                FullName = $"{TEST_ROOT_FOLDER_PATH}\\text.txt",
            };

            var fileProcFactory = new Mock<IFileProcessorFactory>();
            var fileProcessor = new Mock<IFileProcessor>();

            IFileProcessor fp = fileProcessor.Object;
            fileProcFactory.Setup(x => x.TryCreate("txt", out fp)).Returns(true);
            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH, fileProcFactory.Object);

            await fileService.UpdateData(fileData);

            fileProcessor.Verify(x => x.Update(fileData), Times.Once());
        }

        private IFileService CreateFileService(string rootFolderPath)
        {
            var fileProcFactory = new Mock<IFileProcessorFactory>();
            var configManager = new Mock<IConfigurationManager>();

            configManager.Setup(x => x.GetRootFolderPath()).Returns(rootFolderPath);

            return new FileService(fileProcFactory.Object, configManager.Object);
        }

        private IFileService CreateFileService(string rootFolderPath, IFileProcessorFactory fileProcessorFactory)
        {
            var configManager = new Mock<IConfigurationManager>();
            configManager.Setup(x => x.GetRootFolderPath()).Returns(rootFolderPath);

            return new FileService(fileProcessorFactory, configManager.Object);
        }

        private bool CompareTrees(List<FileNodeDto> first, List<FileNodeDto> second)
        {
            if (first == null && second == null)
                return true;

            if (first == null || second == null)
                return false;

            if (first.Count != second.Count)
                return false;

            for (int index = 0; index < first.Count; index++)
            {
                var firstNode = first[index];

                var secondNode = second.SingleOrDefault(x =>
                    x.FullName == firstNode.FullName
                    && x.Name == firstNode.Name
                    && x.IsFolder == firstNode.IsFolder
                    && x.Children.Count == firstNode.Children.Count);

                if (secondNode == null)
                    return false;

                if (firstNode.IsFolder)
                {
                    if (!CompareTrees(firstNode.Children, secondNode.Children))
                        return false;
                }
            }

            return true;
        }

        public static IEnumerable<object[]> GetInvalidExtensionsFileParseOptionsDto()
        {
            yield return new object[] {
                new FileParseOptionsDto()
                {
                    FullName = $"{TEST_ROOT_FOLDER_PATH}\\text",
                }
            };
            yield return new object[] {
                new FileParseOptionsDto()
                {
                    FullName = $"{TEST_ROOT_FOLDER_PATH}\\text.tessgsdtge",
                }
            };
            yield return new object[] {
                new FileParseOptionsDto()
                {
                    FullName = $"{TEST_ROOT_FOLDER_PATH}\\text.....",
                }
            };
        }
    }
}
