using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Commands;
using TAO3.Internal.Commands.GetClipboard;
using TAO3.Internal.Converters;

namespace TAO3.Internal
{
    public class TAO3KernelExtension : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {/*
            Command command = new Command("#!cb", "Copy clipboard value")
            {
                new Argument<DocumentType>("type"),
                new Option<string>(new[] { "-s", "--separator" }, "Csv separator" )
            };

            command.Handler = CommandHandler.Create((string type, string? separator, KernelInvocationContext context) =>
            {
                context.Display(type);
            });
            */
            kernel.AddDirective(new GetClipboardCommand());
            /*
            var clockCommand = new Command("#!clock", "Displays a clock showing the current or specified time.")
                {
                    new Option<int>(new[]{"-o","--hour"},
                                    "The position of the hour hand"),
                    new Option<int>(new[]{"-m","--minute"},
                                    "The position of the minute hand"),
                    new Option<int>(new[]{"-s","--second"},
                                    "The position of the second hand")
                };

            clockCommand.Handler = CommandHandler.Create(
                (int hour, int minute, int second, KernelInvocationContext invocationContext) =>
                {
                    invocationContext.Display($"{hour} {minute} {second}");
                });

            kernel.AddDirective(clockCommand);

            Formatter.Register<DateTime>((date, writer) =>
            {
                writer.Write("<h1>test</h1>");
            }, "text/html");
            */

            return Task.CompletedTask;
        }
    }
}
