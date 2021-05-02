using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Converters;
using Microsoft.DotNet.Interactive.Events;
using Newtonsoft.Json;
using Microsoft.DotNet.Interactive.Commands;
using TAO3.Internal.Interop;
using TAO3.Internal.Extensions;

namespace TAO3.Internal.Commands.CopyResult
{
    internal class CopyResultCommand : Command
    {
        public CopyResultCommand(IInteropOS interop) :
            base("#!copyResult", "Copy returned value to clipboard")
        {
            Add(new Argument<DocumentType>("type", "The document type of the clipboard"));
            Add(new Option<string>(new[] { "-s", "--separator" }, "Separator for CSV format and line separator regex for line format"));

            Handler = CommandHandler.Create((DocumentType type, string separator, KernelInvocationContext context) =>
            {
                Kernel rootKernel = context.HandlingKernel.ParentKernel;
                KernelCommand submitCodeCommand = context.Command.GetRootCommand();

                IDisposable disposable = null!;
                disposable = rootKernel.KernelEvents.Subscribe(
                    onNext: e =>
                    {
                        KernelCommand rootCommand = e.Command.GetRootCommand();

                        if (rootCommand == submitCodeCommand)
                        {
                            if (e is ReturnValueProduced valueProduced)
                            {
                                interop.Clipboard.SetTextAsync(JsonConvert.SerializeObject(valueProduced.Value));
                                disposable.Dispose();
                            }

                            if (e is CommandSucceeded commandSucceeded && commandSucceeded.Command == submitCodeCommand)
                            {
                                disposable.Dispose();
                            }

                            if (e is CommandFailed commandFailed)
                            {
                                disposable.Dispose();
                            }
                        }
                    },
                    onError: e =>
                    {
                        disposable.Dispose();
                    });

                return Task.CompletedTask;
            });
        }
    }
}
