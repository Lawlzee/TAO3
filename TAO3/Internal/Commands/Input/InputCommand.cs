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
using TAO3.IO;
using TAO3.Internal.Extensions;

namespace TAO3.Internal.Commands.Input
{
    internal class InputCommand : Command
    {
        public InputCommand(
            ISourceService sourceService,
            IFormatConverterService formatConverter) 
            : base("#!input", "Get a value from a source and convert it to a C# object")
        {
            AddAlias("#!in");

            sourceService.Events.RegisterChildCommand<ISourceEvent, SourceAddedEvent, SourceRemovedEvent>(
                this,
                x => x.Source.Name,
                evnt =>
                {
                    Command command = new Command(evnt.Source.Name);
                    command.AddAliases(evnt.Source.Aliases);

                    IDisposable formatSubscription = formatConverter.Events.RegisterChildCommand<IConverterEvent, ConverterRegisteredEvent, ConverterUnregisteredEvent>(
                        command,
                        x => x.Converter.Format,
                        formatAdded => CreateConverterCommand(evnt.Source, formatAdded.Converter));

                    return (command, formatSubscription);
                });
        }

        private Command CreateConverterCommand(ISource source, IConverter converter)
        {
            Command command = new Command(converter.Format)
            {
                new Argument<string>("name", "The name of the variable that will contain the deserialized clipboard content"),
                new Option<string>(new[] { "--settings" }, $"Converter settings of type '{converter.SettingsType.FullName}'")
            };

            command.AddAliases(converter.Aliases);

            ConvertionContextProvider convertionContextProvider = new ConvertionContextProvider(converter, source);

            if (converter is IInputConfigurableConverterCommand configurableConverter)
            {
                configurableConverter.Configure(command);
            }

            if (converter is IHandleInputCommand commandHandler)
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
