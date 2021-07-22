using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public class ConverterCommandParameters
    {
        public string? Name { get; set; }
        public string? Settings { get; set; }
        public bool Verbose { get; set; }
        public KernelInvocationContext? Context { get; set; }
    }
}
