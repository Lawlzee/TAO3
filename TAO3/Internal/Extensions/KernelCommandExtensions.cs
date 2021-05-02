using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Extensions
{
    internal static class KernelCommandExtensions
    {
        internal static KernelCommand GetRootCommand(this KernelCommand kernelCommand)
        {
            while (kernelCommand.Parent != null)
            {
                kernelCommand = kernelCommand.Parent;
            }

            return kernelCommand;
        }
    }
}
