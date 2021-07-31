using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Extensions
{
    internal static class SubmitCodeExtensions
    {
        private static readonly PropertyInfo _kernelNameDirectiveNodeProperty = typeof(SubmitCode)
            .GetProperty("KernelNameDirectiveNode", BindingFlags.NonPublic | BindingFlags.Instance)!;

        public static KernelNameDirectiveNode GetKernelNameDirectiveNode(this SubmitCode submitCode)
        {
            return (KernelNameDirectiveNode)_kernelNameDirectiveNodeProperty.GetValue(submitCode)!;
        }
    }
}
