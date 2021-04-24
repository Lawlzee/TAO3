using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3
{
    public class TAO3KernelExtension : IKernelExtension
    {
        public static string Test { get; set; } = "ALLOmiam";

        public Task OnLoadAsync(Kernel kernel)
        {
            Command command = new Command("#!cb", "Copy clipboard value")
            {
                new Argument<string>("type")
            };

            command.Handler = CommandHandler.Create((string type, KernelInvocationContext context) =>
            {
                context.Display(type);
            });

            kernel.AddDirective(command);

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


            return Task.CompletedTask;
        }
    }
}
