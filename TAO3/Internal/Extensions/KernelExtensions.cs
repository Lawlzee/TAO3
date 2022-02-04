using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using System.Reactive;
using System.Reactive.Linq;

namespace TAO3.Internal.Extensions;

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

    public static async Task ScheduleSubmitCodeAsync(this Kernel kernel, string code)
    {
        //Hack, submiting code of a kernel doesn't work in the CompositeKernel
        //and submiting a command (ex. #!cell myCell) doesn't work in a child kernel (ex. CSharpKernel)
        code = $"#!{kernel.Name}\r\n" + code;

        if (KernelInvocationContext.Current == null || KernelInvocationContext.Current.IsComplete)
        {
            await kernel.ParentKernel.SubmitCodeAsync(code);
        }
        else
        {
            KernelInvocationContext context = KernelInvocationContext.Current;

            IDisposable disposable = null!;
            disposable = kernel.ParentKernel.KernelEvents
                .SelectMany(async e =>
                {
                    KernelCommand rootCommand = e.Command.GetRootCommand();

                    if (context.Command == rootCommand)
                    {
                        if (e is CommandSucceeded succeeded)
                        {
                            disposable.Dispose();
                            await kernel.ParentKernel.SubmitCodeAsync(code);
                        }

                        if (e is CommandFailed failed)
                        {
                            disposable.Dispose();
                        }
                    }

                    return Unit.Default;
                })
                .Subscribe();
        }
    }
}
