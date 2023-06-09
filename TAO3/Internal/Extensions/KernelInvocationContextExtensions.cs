using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;

namespace TAO3.Internal.Extensions;

internal static class KernelInvocationContextExtensions
{
    public static CSharpKernel GetCSharpKernel(this KernelInvocationContext context)
    {
        return (CSharpKernel)context.HandlingKernel.FindKernelByName("csharp");
    }
}
