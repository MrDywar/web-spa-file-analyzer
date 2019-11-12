using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FilePrcossor
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        public const string TextFileExtension = "txt";

        public bool IsExtensionSupported(string fileExtension)
        {
            return string.Equals(TextFileExtension, fileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public IFileProcessor Create(string fileExtension)
        {
            switch (fileExtension?.ToLower())
            {
                case TextFileExtension:
                    return new TextFileProcessor();

                default:
                    throw new BusinessLogicException(
                        string.Format(Resources.ErrorMessages.FileProcessorNotSupported, fileExtension));
            }
        }

        public bool TryCreate(string fileExtension, out IFileProcessor fileProcessor)
        {
            if (!IsExtensionSupported(fileExtension))
            {
                fileProcessor = null;
                return false;
            }

            fileProcessor = Create(fileExtension);

            return true;
        }
    }
}
