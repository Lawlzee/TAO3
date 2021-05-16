using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.InputSources
{
    public interface IInputSource
    {
        string Name { get; }
        Task<string> GetTextAsync(KernelInvocationContext context);
    }
}
