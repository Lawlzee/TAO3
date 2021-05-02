using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Commands.CopyResult;
using TAO3.Internal.Commands.GetClipboard;
using TAO3.Internal.Commands.Macro;
using TAO3.Internal.Interop;
using WindowsHook;

namespace TAO3.Internal
{
    public class TAO3KernelExtension : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {
            Debugger.Launch();

            IInteropOS interop = InteropFactory.Create();

            kernel.AddDirective(new MacroCommand(interop));
            kernel.AddDirective(new GetClipboardCommand(interop));
            kernel.AddDirective(new CopyResultCommand(interop));

            return Task.CompletedTask;
        }
    }
}
