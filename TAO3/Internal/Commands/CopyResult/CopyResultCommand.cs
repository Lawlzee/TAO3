using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive.Events;
using Newtonsoft.Json;
using Microsoft.DotNet.Interactive.Commands;
using TAO3.Internal.Interop;
using TAO3.Internal.Extensions;
using TAO3.Internal.Services;
using TAO3.Converters;
using Microsoft.DotNet.Interactive.CSharp;

namespace TAO3.Internal.Commands.CopyResult
{
    internal class CopyResultCommand : Command
    {
        public CopyResultCommand(IInteropOS interop, IFormatConverterService formatConverterService) :
            base("#!copyResult", "Copy returned value to clipboard")
        {
            formatConverterService.Events.Subscribe(e =>
            {
                IConverter converter = e.Converter;
                if (e is ConverterRegisteredEvent registeredEvent)
                {
                    Command command = new Command(converter.Format)
                    {
                        new Option<string>(new[] {"-s", "--settings" }, $"Converter settings of type '{converter.SettingsType.FullName}'")
                    };

                    command.Handler = CommandHandler.Create((string settings, KernelInvocationContext context) =>
                    {
                        object? settingsInstance = null;

                        CSharpKernel cSharpKernel = (CSharpKernel)context.HandlingKernel.FindKernel("csharp");
                        if (settings != string.Empty && !cSharpKernel.TryGetVariable(settings, out settingsInstance))
                        {
                            context.Fail(new ArgumentException(), $"The variable '{settings}' was not found");
                            return;
                        }

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
                                        string resultText = converter.Serialize(valueProduced.Value, settingsInstance);
                                        interop.Clipboard.SetTextAsync(resultText);
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
                    });

                    Add(command);
                }
            });
        }
    }
}
