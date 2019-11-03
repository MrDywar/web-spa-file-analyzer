using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class FileNodeDto
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsFolder { get; set; }
        public List<FileNodeDto> Children { get; set; } = new List<FileNodeDto>();
    }
}
