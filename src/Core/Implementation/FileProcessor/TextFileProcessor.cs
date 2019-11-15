using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dto;
using Core.FileProcessor;

namespace Core.Implementation.FileProcessor
{
    public class TextFileProcessor : IFileProcessor
    {
        private const int DefaultBufferSize = 4096;
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public async Task<FileDataDto> Read(FileParseOptionsDto parseOptions)
        {
            CheckFileExist(parseOptions.FullName);

            var result = new FileDataDto()
            {
                FullName = parseOptions.FullName,
                Delimiter = parseOptions.Delimiter
            };

            using (var stream = new FileStream(parseOptions.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            using (var reader = new StreamReader(stream, Encoding.Default))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    result.Rows.Add(line.Split(new string[] { parseOptions.Delimiter }, StringSplitOptions.None));
                }
            }

            if (result.Rows.Count == 0)
                return result;

            if (parseOptions.HasHeaders)
            {
                result.Headers.AddRange(result.Rows[0]);
                result.Rows.RemoveAt(0);
            }
            else
            {
                var maxColumnsCount = result.Rows.Max(x => x.Length);
                result.Headers.AddRange(Enumerable.Range(0, maxColumnsCount).Select(x => $"col_{x}"));
            }

            return result;
        }

        public async Task Update(FileDataDto fileData)
        {
            CheckFileExist(fileData.FullName);

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine(string.Join(fileData.Delimiter, fileData.Headers));

            foreach (var row in fileData.Rows)
                strBuilder.AppendLine(string.Join(fileData.Delimiter, row));

            using (StreamWriter writer = new StreamWriter(fileData.FullName, false, Encoding.Default))
            {
                await writer.WriteAsync(strBuilder.ToString());
            }
        }

        private static void CheckFileExist(string fileFullName)
        {
            if (!File.Exists(fileFullName))
                throw new FileNotFoundException(string.Format(Resources.ErrorMessages.FileNotFound, fileFullName));
        }
    }
}
