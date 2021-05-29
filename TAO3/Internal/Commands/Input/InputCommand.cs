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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TAO3.Converters;
using TAO3.InputSources;

namespace TAO3.Internal.Commands.Input
{
    internal class InputCommand : Command
    {
        public InputCommand(
            IInputSourceService inputSource,
            IFormatConverterService formatConverter) 
            : base("#!in", "Get a value from a source and convert it to C# object")
        {
            inputSource.Events.Subscribe(inputSourceEvent =>
            {
                Command inputSourceCommand = new Command(inputSourceEvent.InputSource.Name);

                Add(inputSourceCommand);

                formatConverter.Events.Subscribe(formatConveterEvenet =>
                {
                    if (formatConveterEvenet is ConverterRegisteredEvent registeredEvent)
                    {
                        AddConverterCommand(inputSourceCommand, inputSourceEvent.InputSource, formatConveterEvenet.Converter);
                    }
                });
            });
        }

        private void AddConverterCommand(Command parentCommand, IInputSource inputSource, IConverter converter)
        {
            Command command = new Command(converter.Format)
            {
                new Argument<string>("name", "The name of the variable that will contain the deserialized clipboard content"),
                new Option<string>(new[] { "--settings" }, $"Converter settings of type '{converter.SettingsType.FullName}'")
            };

            ConvertionContextProvider convertionContextProvider = new ConvertionContextProvider(converter, inputSource);

            if (converter is IConfigurableConverter configurableConverter)
            {
                command.Add(new Option(new[] { "-v", "--verbose" }, "Print debugging information"));

                configurableConverter.ConfigureCommand(
                    command,
                    convertionContextProvider);
            }
            else
            {
                command.Handler = CommandHandler.Create(async (string name, string settings, KernelInvocationContext context) =>
                {
                    IConverterContext<object> convertionContext = convertionContextProvider.Invoke(name, settings, verbose: false, context);
                    await convertionContext.DefaultHandle();
                });
            }

            parentCommand.Add(command);
        }
    }
}
