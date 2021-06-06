using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Extensions
{
    internal static class KernelExtensions
    {
        public static void ScheduleSubmitCode(this Kernel kernel, string code)
        {
            if (KernelInvocationContext.Current == null || KernelInvocationContext.Current.IsComplete)
            {
                kernel.SubmitCodeAsync(code).Wait();
            }
            else
            {
                kernel.DeferCommand(new SubmitCode(code));
            }
        }
    }
}
