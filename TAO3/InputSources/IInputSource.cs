using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Interop;

namespace TAO3.InputSources
{
    public interface IInputSource
    {
        string Name { get; }
        Task<string> GetText(string source, KernelInvocationContext context);
    }
}
