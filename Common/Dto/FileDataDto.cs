using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class FileDataDto
    {
        public string FullName { get; set; }
        public string Delimiter { get; set; }
        public List<string> Headers { get; set; } = new List<string>();
        public List<string[]> Rows { get; set; } = new List<string[]>();
    }
}
