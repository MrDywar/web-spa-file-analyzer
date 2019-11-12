using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FileProcessor
{
    public interface IFileProcessorFactory
    {
        bool IsExtensionSupported(string fileExtension);
        IFileProcessor Create(string fileExtension);
        bool TryCreate(string fileExtension, out IFileProcessor fileProcessor);
    }
}
