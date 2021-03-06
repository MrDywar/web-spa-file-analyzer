﻿using Common.Dto;
using Common.Exceptions;
using Core.FileProcessor;
using Core.Implementation;
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
        public async Task GetAll_DefaultRootFolder_ReturnsFolderTree()
        {
            var path = TestHelper.PATH_TO_TESTFILES;
            var expectedTree = new List<FileNodeDto>()
            {
                new FileNodeDto($"{path}\\A", "A", true)
                {
                    Children = new List<FileNodeDto>()
                    {
                        new FileNodeDto($"{path}\\A\\A1", "A1", true)
                        {
                            Children = new List<FileNodeDto>()
                            {
                                new FileNodeDto($"{path}\\A\\A1\\A1.txt", "A1.txt", false)
                            }
                        },
                        new FileNodeDto($"{path}\\A\\A.txt", "A.txt", false)
                    }
                },
                new FileNodeDto($"{path}\\empty.txt", "empty.txt", false),
                new FileNodeDto($"{path}\\tab_file.txt", "tab_file.txt", false)
            };

            var fileService = CreateFileService(path);

            var tree = await fileService.GetAll();

            Assert.IsTrue(CompareTrees(tree, expectedTree));
        }

        [TestMethod]
        public async Task GetData_InvalidRootPath_ThrowBusinessLogicException()
        {
            var parseOptions = new FileParseOptionsDto()
            {
                FullName = @"C:\f2\text.txt",
            };

            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH);

            Func<Task<FileDataDto>> act = () => fileService.GetData(parseOptions);

            await Assert.ThrowsExceptionAsync<BusinessLogicException>(act);
        }

        [TestMethod]
        [DynamicData(nameof(GetFileParseOptionsDtos_Invalid), DynamicDataSourceType.Method)]
        public async Task GetData_InvalidDto_ThrowValidationException(FileParseOptionsDto value)
        {
            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH);

            Func<Task<FileDataDto>> act = () => fileService.GetData(value);

            await Assert.ThrowsExceptionAsync<ValidationException>(act);
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
                FullName = $"{TEST_ROOT_FOLDER_PATH}\\text.txt"
            };

            var fileProcFactory = new Mock<IFileProcessorFactory>();
            var fileProcessor = new Mock<IFileProcessor>();

            fileProcessor.Setup(x => x.Read(parseOptions)).Returns(Task.FromResult(expectedData));
            fileProcFactory.Setup(x => x.Create("txt")).Returns(fileProcessor.Object);

            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH, fileProcFactory.Object);

            var resultFileData = await fileService.GetData(parseOptions);

            Assert.ReferenceEquals(resultFileData, expectedData);
        }

        [TestMethod]
        public async Task UpdateData_InvalidRootPath_ThrowBusinessLogicException()
        {
            var fileData = new FileDataDto()
            {
                FullName = @"C:\text.txt",
                Headers = new List<string>() { "test" }
            };
            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH);

            Func<Task> act = () => fileService.UpdateData(fileData);

            await Assert.ThrowsExceptionAsync<BusinessLogicException>(act);
        }

        [TestMethod]
        [DynamicData(nameof(GetFileDataDtos_Invalid), DynamicDataSourceType.Method)]
        public async Task UpdateData_InvalidDto_ThrowValidationException(FileDataDto value)
        {
            var fileService = CreateFileService(TEST_ROOT_FOLDER_PATH);

            Func<Task> act = () => fileService.UpdateData(value);

            await Assert.ThrowsExceptionAsync<ValidationException>(act);
        }

        [TestMethod]
        public async Task UpdateData_ValidFile_UpdateCalledOnFileProcessor()
        {
            var fileData = new FileDataDto()
            {
                FullName = $"{TEST_ROOT_FOLDER_PATH}\\text.txt",
                Headers = new List<string>() { "test" }
            };

            var fileProcFactory = new Mock<IFileProcessorFactory>();
            var fileProcessor = new Mock<IFileProcessor>();

            fileProcFactory.Setup(x => x.Create("txt")).Returns(fileProcessor.Object);
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

        public static IEnumerable<object[]> GetFileParseOptionsDtos_Invalid()
        {
            yield return new object[] { null };
            yield return new object[] {
                new FileParseOptionsDto() { FullName = null }
            };
        }

        public static IEnumerable<object[]> GetFileDataDtos_Invalid()
        {
            yield return new object[] { null };
            yield return new object[] {
                new FileDataDto() { FullName = null }
            };
            yield return new object[] {
                new FileDataDto() { FullName = "a" }
            };
        }
    }
}
