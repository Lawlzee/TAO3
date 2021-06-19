using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Commands.Macro
{
    public class SubmitJsCodeCommand : KernelCommand
    {
        public string Code { get; }

        public SubmitJsCodeCommand(string code) : base("javascript")
        {
            Code = code;
        }
    }
}
