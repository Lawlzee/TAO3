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
using TAO3.Internal.Interop;
using TAO3.Internal.Services;

namespace TAO3.Internal.Commands.GetClipboard
{
    internal class GetClipboardCommand : Command
    {
        public GetClipboardCommand(IInteropOS interop, IFormatConverterService formatConverterService) :
            base("#!getClipboard", "Copy clipboard value")
        {
            formatConverterService.Events.Subscribe(e =>
            {
                IConverter converter = e.Converter;
                if (e is ConverterRegisteredEvent registeredEvent)
                {
                    Command command = new Command(converter.Format)
                    {
                        new Argument<string>("name", "The name of the variable that will contain the deserialized clipboard content"),
                        new Option<string>(new[] { "--settings" }, $"Converter settings of type '{converter.SettingsType.FullName}'")
                    };

                    ConvertionContextProvider convertionContextProvider = new ConvertionContextProvider(
                        converter,
                        async () => await interop.Clipboard.GetTextAsync() ?? string.Empty);

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

                    Add(command);
                }
            });
        }
    }
}
