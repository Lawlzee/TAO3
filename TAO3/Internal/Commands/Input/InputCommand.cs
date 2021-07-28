using Microsoft.CodeAnalysis.CSharp;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TAO3.Converters;
using TAO3.InputSources;
using TAO3.Internal.Extensions;

namespace TAO3.Internal.Commands.Input
{
    internal class InputCommand : Command
    {
        public InputCommand(
            IInputSourceService inputSourceService,
            IFormatConverterService formatConverter) 
            : base("#!in", "Get a value from a source and convert it to C# object")
        {
            inputSourceService.Events.RegisterChildCommand<IInputSourceEvent, InputSourceAddedEvent, InputSourceRemovedEvent>(
                this,
                x => x.InputSource.Name,
                evnt =>
                {
                    Command command = new Command(evnt.InputSource.Name);

                    IDisposable formatSubscription = formatConverter.Events.RegisterChildCommand<IConverterEvent, ConverterRegisteredEvent, ConverterUnregisteredEvent>(
                        command,
                        x => x.Converter.Format,
                        (formatAdded) => CreateConverterCommand(evnt.InputSource, formatAdded.Converter));

                    return (command, formatSubscription);
                });
        }

        private Command CreateConverterCommand(IInputSource inputSource, IConverter converter)
        {
            Command command = new Command(converter.Format)
            {
                new Argument<string>("name", "The name of the variable that will contain the deserialized clipboard content"),
                new Option<string>(new[] { "--settings" }, $"Converter settings of type '{converter.SettingsType.FullName}'")
            };

            ConvertionContextProvider convertionContextProvider = new ConvertionContextProvider(converter, inputSource);

            if (converter is IConfigurableConverter configurableConverter)
            {
                configurableConverter.Configure(command);
            }

            if (converter is IHandleCommand commandHandler)
            {
                command.Add(new Option(new[] { "-v", "--verbose" }, "Print debugging information"));

                command.Handler = commandHandler.CreateHandler(convertionContextProvider);
            }
            else
            {
                command.Handler = CommandHandler.Create(async (string name, string settings, KernelInvocationContext context) =>
                {
                    try
                    {
                        IConverterContext<object> convertionContext = convertionContextProvider.Invoke(name, settings, verbose: false, context);
                        await convertionContext.DefaultHandleCommandAsync();
                    }
                    catch (Exception ex)
                    {
                        ex.Display();
                    }
                });
            }

            return command;
        }
    }
}
