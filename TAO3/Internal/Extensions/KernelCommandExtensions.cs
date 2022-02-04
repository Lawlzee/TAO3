using Microsoft.DotNet.Interactive.Commands;

namespace TAO3.Internal.Extensions;

internal static class KernelCommandExtensions
{
    public static KernelCommand GetRootCommand(this KernelCommand kernelCommand)
    {
        while (kernelCommand.Parent != null)
        {
            kernelCommand = kernelCommand.Parent;
        }

        return kernelCommand;
    }
}
