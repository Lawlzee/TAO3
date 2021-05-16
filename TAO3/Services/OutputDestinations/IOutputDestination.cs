using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.OutputDestinations
{
    public interface IOutputDestination
    {
        public string Name { get; }
        public Task SetTextAsync(string text, KernelInvocationContext context);
    }
}
