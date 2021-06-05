using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Extensions
{
    internal static class KernelInvocationContextExtensions
    {
        public static CSharpKernel GetCSharpKernel(this KernelInvocationContext context)
        {
            return (CSharpKernel)context.HandlingKernel.FindKernel("csharp");
        }
    }
}
