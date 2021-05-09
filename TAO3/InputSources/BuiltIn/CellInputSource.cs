using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.InputSources
{
    internal class CellInputSource : IInputSource
    {
        public string Name => "cell";

        public Task<string> GetText(string source, KernelInvocationContext context)
        {
            string code = ((SubmitCode)context.Command).Code;
            return Task.Run(() => string.Join(
                Environment.NewLine, 
                code.Split(Environment.NewLine, StringSplitOptions.None)
                    .Skip(1)));
        }
    }
}
