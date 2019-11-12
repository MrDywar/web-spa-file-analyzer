using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Dto;

namespace Core.FilePrcossor
{
    public class TextFileProcessor : IFileProcessor
    {
        public Task<FileDataDto> Read(FileParseOptionsDto parseOptions)
        {
            CheckFileExist(parseOptions.FullName);

            var result = new FileDataDto()
            {
                FullName = parseOptions.FullName,
                Delimiter = parseOptions.Delimiter
            };

            foreach (string line in File.ReadLines(parseOptions.FullName, Encoding.Default))
                result.Rows.Add(line.Split(new string[] { parseOptions.Delimiter }, StringSplitOptions.None));

            if (result.Rows.Count == 0)
                return Task.FromResult(result);

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

            return Task.FromResult(result);
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
