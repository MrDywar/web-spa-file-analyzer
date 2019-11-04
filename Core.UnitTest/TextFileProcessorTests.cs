using Common.Dto;
using Core.FilePrcossor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.UnitTest
{
    [TestClass]
    public class TextFileProcessorTests
    {
        private IFileProcessor _fileProcessor;

        private const string EMPTY_TEXTFILE_PATH = @"TestFiles\empty.txt";
        private const string EMPTY_TEXTFILE_NAME = "empty.txt";

        private const string TAB_TEXTFILE_PATH = @"TestFiles\tab_file.txt";
        private const string TAB_TEXTFILE_NAME = "tab_file.txt";

        [TestInitialize]
        public void Setup()
        {
            _fileProcessor = new TextFileProcessor();
        }

        [TestMethod]
        [DeploymentItem(EMPTY_TEXTFILE_PATH)]
        public async Task Read_EmptyTextFile_ReturnsEmptyFileData()
        {
            var parseOptions = new FileParseOptionsDto()
            {
                FullName = EMPTY_TEXTFILE_NAME,
                Delimiter = " ",
                HasHeaders = false
            };

            var expectedData = new FileDataDto()
            {
                FullName = EMPTY_TEXTFILE_NAME,
                Delimiter = " "
            };

            var resultData = await _fileProcessor.Read(parseOptions);

            Assert.IsTrue(IsFileDataDtoEquals(resultData, expectedData));
        }

        [TestMethod]
        [DeploymentItem(TAB_TEXTFILE_PATH)]
        public async Task Read_TextFileWithHeaders_ReturnsFileData()
        {
            var parseOptions = new FileParseOptionsDto()
            {
                FullName = TAB_TEXTFILE_NAME,
                Delimiter = "\t",
                HasHeaders = true
            };

            var expectedData = new FileDataDto()
            {
                FullName = TAB_TEXTFILE_NAME,
                Delimiter = "\t",
                Headers = new List<string>() { "Name", "Site", "Email" },
                Rows = new List<string[]>()
                    {
                        new string[] { "Betty J. Brown", "OEMJobs.com", "BettyJBrown@armyspy.com" },
                        new string[] { "John D. Ginter", "keepclicker.com", "JohnDGinter@rhyta.com" },
                        new string[] { "Roger I. Clinton", "tastrons.com", "RogerIClinton@jourrapide.com" }
                    }
            }; ;

            var resultData = await _fileProcessor.Read(parseOptions);

            Assert.IsTrue(IsFileDataDtoEquals(resultData, expectedData));
        }

        [TestMethod]
        [DeploymentItem(TAB_TEXTFILE_PATH)]
        public async Task Read_TextFileWithoutHeaders_ReturnsFileDataWithCustomHeaders()
        {
            var parseOptions = new FileParseOptionsDto()
            {
                FullName = TAB_TEXTFILE_NAME,
                Delimiter = "\t",
                HasHeaders = false
            };

            var expectedData = new FileDataDto()
            {
                FullName = TAB_TEXTFILE_NAME,
                Delimiter = "\t",
                Headers = new List<string>() { "col_0", "col_1", "col_2" },
                Rows = new List<string[]>()
                    {
                        new string[] { "Name", "Site", "Email" },
                        new string[] { "Betty J. Brown", "OEMJobs.com", "BettyJBrown@armyspy.com" },
                        new string[] { "John D. Ginter", "keepclicker.com", "JohnDGinter@rhyta.com" },
                        new string[] { "Roger I. Clinton", "tastrons.com", "RogerIClinton@jourrapide.com" }
                    }
            };

            var resultData = await _fileProcessor.Read(parseOptions);

            Assert.IsTrue(IsFileDataDtoEquals(resultData, expectedData));
        }

        [TestMethod]
        public async Task Read_NonexistentTextFile_ThrowFileNotFoundException()
        {
            var parseOptions = new FileParseOptionsDto()
            {
                FullName = Guid.NewGuid().ToString(),
                Delimiter = " ",
                HasHeaders = false
            };

            Func<Task> act = () => _fileProcessor.Read(parseOptions);

            await Assert.ThrowsExceptionAsync<FileNotFoundException>(act);
        }

        [TestMethod]
        [DeploymentItem(TAB_TEXTFILE_PATH)]
        public async Task Update_TextFile_OverwriteFileAndReturnsTask()
        {
            var firstDataToWrite = new FileDataDto()
            {
                FullName = TAB_TEXTFILE_NAME,
                Delimiter = "\t",
                Headers = new List<string>() { "1", "2" },
                Rows = new List<string[]>()
                {
                    new string[] { "A", "B" }
                }
            };

            var secondDataToWrite = new FileDataDto()
            {
                FullName = TAB_TEXTFILE_NAME,
                Delimiter = "\t",
                Headers = new List<string>() { "COL_1", "COL_2" },
                Rows = new List<string[]>()
                {
                    new string[] { "A", "B" },
                    new string[] { "A1", "B1" }
                }
            };

            var parseOptions = new FileParseOptionsDto()
            {
                FullName = TAB_TEXTFILE_NAME,
                Delimiter = "\t",
                HasHeaders = true
            };

            await _fileProcessor.Update(firstDataToWrite);
            await _fileProcessor.Update(secondDataToWrite);
            var resultData = await _fileProcessor.Read(parseOptions);

            Assert.IsTrue(IsFileDataDtoEquals(resultData, secondDataToWrite));
        }

        [TestMethod]
        public async Task Update_NonexistentTextFile_ThrowFileNotFoundException()
        {
            var fileData = new FileDataDto()
            {
                FullName = Guid.NewGuid().ToString(),
                Delimiter = " ",
            };

            Func<Task> act = () => _fileProcessor.Update(fileData);

            await Assert.ThrowsExceptionAsync<FileNotFoundException>(act);
        }

        private bool IsFileDataDtoEquals(FileDataDto first, FileDataDto second)
        {
            if (first == null && second == null)
                return true;

            if (first == null || second == null)
                return false;

            if (first.FullName != second.FullName
                || first.Delimiter != second.Delimiter
                || !first.Headers.SequenceEqual(second.Headers)
                || first.Rows.Count != second.Rows.Count)
                return false;

            for (int index = 0; index < first.Rows.Count; index++)
            {
                if (!first.Rows[index].SequenceEqual(second.Rows[index]))
                    return false;
            }

            return true;
        }
    }
}
