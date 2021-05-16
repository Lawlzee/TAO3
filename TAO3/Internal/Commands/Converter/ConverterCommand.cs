using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TAO3.Converters;
using TAO3.Services;

namespace TAO3.Internal.Commands.Converter
{
    internal class ConverterCommand : Command
    {
        public ConverterCommand(IFormatConverterService formatConverter) 
            : base("#!converter", "Manage the format converters")
        {
            Add(new RegisterConverterCommand(formatConverter));
            Add(new UnregisterConverterCommand(formatConverter));
        }
    }

    internal class RegisterConverterCommand : Command
    {
        public RegisterConverterCommand(IFormatConverterService formatConverter)
            : base("add", "Add a new format converter")
        {
            Add(new Argument<string>("converter", "The name of the variable containing an instance of TAO3.Converters.IConveter to be registered"));

            Handler = CommandHandler.Create((string converter, KernelInvocationContext context) =>
            {
                CSharpKernel cSharpKernel = (CSharpKernel)context.HandlingKernel.FindKernel("csharp");
                if (cSharpKernel.TryGetVariable(converter, out IConverter converterInstance))
                {
                    formatConverter.Register(converterInstance);
                    context.Display($"{converterInstance.Format} Converter '{converterInstance.GetType().FullName}' registered", null);
                    return;
                }

                context.Fail(new ArgumentException(), "Invalid argument 'conveter'");
            });
        }
    }

    internal class UnregisterConverterCommand : Command
    {
        public UnregisterConverterCommand(IFormatConverterService formatConverter)
            : base("remove", "remove a format converter")
        {
            Add(new Argument<string>("format", "The name of the format to remove"));

            Handler = CommandHandler.Create((string format, KernelInvocationContext context) =>
            {
                formatConverter.UnregisterConverter(format);
            });
        }
    }

}
