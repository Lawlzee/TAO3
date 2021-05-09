using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.InputSources
{
    internal class FileInputSource : IInputSource
    {
        public string Name => "file";

        public async Task<string> GetText(string source, KernelInvocationContext context)
        {
            return await File.ReadAllTextAsync(source);
        }
    }
}
