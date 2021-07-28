using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Commands;
using TAO3.Internal.Extensions;
using TAO3.Converters;
using Microsoft.DotNet.Interactive.CSharp;
using TAO3.OutputDestinations;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive;

namespace TAO3.Internal.Commands.Output
{
    internal class OutputCommand : Command
    {
        public OutputCommand(IOutputDestinationService outputDestination, IFormatConverterService formatConverter)
            : base("#!out", "Copy returned value to clipboard")
        {
            outputDestination.Events.RegisterChildCommand<IOutputDestinationEvent, OutputDestinationAddedEvent, OutputDestinationRemovedEvent>(
                this,
                x => x.OutputDestination.Name,
                evnt =>
                {
                    Command command = new Command(evnt.OutputDestination.Name);

                    IDisposable formatSubscription = formatConverter.Events.RegisterChildCommand<IConverterEvent, ConverterRegisteredEvent, ConverterUnregisteredEvent>(
                        command,
                        x => x.Converter.Format,
                        (formatAddedEvent) => CreateConverterCommand(evnt.OutputDestination, formatAddedEvent.Converter));

                    return (command, formatSubscription);
                });
        }

        private Command CreateConverterCommand(IOutputDestination outputDestination, IConverter converter)
        {
            Command command = new Command(converter.Format)
            {
                new Option<string>(new[] {"-s", "--settings" }, $"Converter settings of type '{converter.SettingsType.FullName}'")
            };

            command.Handler = CommandHandler.Create((string settings, KernelInvocationContext context) =>
            {
                object? settingsInstance = null;

                CSharpKernel cSharpKernel = context.GetCSharpKernel();
                if (settings != string.Empty && !cSharpKernel.TryGetVariable(settings, out settingsInstance))
                {
                    context.Fail(new ArgumentException(), $"The variable '{settings}' was not found");
                    return;
                }

                Kernel rootKernel = context.HandlingKernel.ParentKernel;
                KernelCommand submitCodeCommand = context.Command.GetRootCommand();

                IDisposable disposable = null!;
                disposable = rootKernel.KernelEvents
                    .SelectMany(async e =>
                    {
                        KernelCommand rootCommand = e.Command.GetRootCommand();

                        if (rootCommand == submitCodeCommand)
                        {
                            if (e is ReturnValueProduced valueProduced)
                            {
                                try
                                {
                                    string resultText = converter.Serialize(valueProduced.Value, settingsInstance);
                                    await outputDestination.SetTextAsync(resultText);
                                }
                                catch (Exception ex)
                                {
                                    ex.Display();
                                }
                                finally
                                {
                                    disposable.Dispose();
                                }
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

                        return Unit.Default;
                    })
                    .Subscribe();
            });

            return command;
        }
    }
}
