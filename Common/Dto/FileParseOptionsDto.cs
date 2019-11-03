using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class FileParseOptionsDto
    {
        public string FullName { get; set; }
        public bool HasHeaders { get; set; }
        public string Delimiter { get; set; }
    }
}
