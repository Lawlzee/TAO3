using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.IO
{
    internal class CellSource : ISource
    {
        public string Name => "cell";

        public IReadOnlyList<string> Aliases => new[] { "Cell" };

        public Task<string> GetTextAsync()
        {
            string code = ((SubmitCode)KernelInvocationContext.Current.Command).Code;
            return Task.Run(() => string.Join(
                Environment.NewLine, 
                code.Split(Environment.NewLine, StringSplitOptions.None)
                    .Skip(1)));
        }
    }
}
