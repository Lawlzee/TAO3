using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Connection;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;
using TAO3.Keyboard;
using TAO3.Macro;
using TAO3.Toast;
using WindowsHook;

namespace TAO3.Internal.Commands.Macro
{
    internal class MacroCommand : Command
    {
        private MacroCommand(
            IMacroService macroService,
            ProxyKernel javaScriptKernel,
            HtmlKernel htmlKernel)
            : base("#!macro", "Add a macro that run the code in the cell")
        {
            Add(new Argument<string>("shortcut", "Ex. CTRL+SHIFT+1"));
            Add(new Option<string>(new[] { "-n", "--name" }, "Macro name"));
            Add(new Option(new[] { "-s", "--silent" }, "Disable the toast notifications"));

            Handler = CommandHandler.Create(async (string shortcut, string name, bool silent, KernelInvocationContext context) =>
            {
                SubmitCode originalCommand = (SubmitCode)context.Command;
                string code = RemoveMacroCommand(originalCommand.Code);

                string macroName = string.IsNullOrEmpty(name)
                    ? shortcut
                    : name;

                TAO3Macro macro = new TAO3Macro(
                    macroName,
                    shortcut,
                    code,
                    originalCommand.TargetKernelName,
                    !silent);

                string cellId = $"__internal_cell_{Guid.NewGuid():N}";

                CommandFailed? commandFailed = null;
                List<ReturnValueProduced> valuesProduced = new List<ReturnValueProduced>();

                IDisposable subscription = null!;
                subscription = macroService.Events
                    .Where(evnt => evnt.Macro == macro)
                    .SelectMany(async evnt =>
                    {
                        if (evnt is MacroRemoved)
                        {
                            subscription.Dispose();
                        }

                        if (evnt is MacroValueProduced valueProduced)
                        {
                            valuesProduced.Add(valueProduced.ReturnValueProduced);
                        }

                        if (evnt is MacroExecutionFailed failed)
                        {
                            commandFailed = failed.CommandFailed;
                        }

                        if (evnt is MacroExecutionCompleted completed)
                        {
                            try
                            {
                                await PrintCellResultAsync(macro, cellId, commandFailed, valuesProduced);
                            }
                            finally
                            {
                                commandFailed = null;
                                valuesProduced.Clear();
                            }
                        }

                        return Unit.Default;
                    })
                    .Subscribe();
                
                macroService.Add(macro);

                await htmlKernel.SubmitCodeAsync($@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8' />
                        <script type='text/javascript'>
                            function {cellId}_Print(values) {{
                                var cell = document.getElementById(""{cellId}"");
                                cell.innerHTML = """";
                                for (let i = 0; i < values.length; i++)
                                {{
                                    let div = document.createElement('div');
                                    div.innerHTML = values[i].trim();
                                    cell.appendChild(div);
                                }}
                            }}
                        </script>
                    </head>
                    <body>
                        <div style='margin: -5px' id='{cellId}'>Macro was registered successfully. Use {shortcut} to run the macro.</div>
                    </body>
                    </html>");

                context.Complete(context.Command);
            });

            async Task PrintCellResultAsync(
                TAO3Macro macro,
                string cellId,
                CommandFailed? commandFailed,
                List<ReturnValueProduced> valuesProduced)
            {
                string[] body = (valuesProduced.Count > 0, commandFailed != null, macro.ToastNotificationOnRun) switch
                {
                    (true, _, _) => valuesProduced.SelectMany(x => x.FormattedValues).Select(x => x.Value).ToArray(),
                    (_, true, _) => FormatCommandFailedBody(),
                    (_, _, false) => new[] { $"{macro.Name} ran successfully!" },
                    _ => Array.Empty<string>()
                };

                string json = JsonConvert.SerializeObject(body);
                await javaScriptKernel.SendAsync(new SubmitJsCodeCommand($@"{cellId}_Print({json})"));

                string[] FormatCommandFailedBody()
                {
                    string[] body = Regex.Split(commandFailed!.Message, @"\r\n|\r|\n");

                    if (commandFailed!.Exception == null)
                    {
                        return body;
                    }

                    return body.Concat(Regex.Split(commandFailed!.Exception.ToString(), @"\r\n|\r|\n")).ToArray();
                }
            }
        }

        public static async Task<MacroCommand> CreateAsync(
            IMacroService macroService,
            ProxyKernel javaScriptKernel,
            HtmlKernel htmlKernel)
        {
            await RegisterSendJavascriptCodeCommandAsync(javaScriptKernel);
            return new MacroCommand(macroService, javaScriptKernel, htmlKernel);
        }

        private static async Task RegisterSendJavascriptCodeCommandAsync(ProxyKernel javaScriptKernel)
        {
            javaScriptKernel.RegisterCommandType<SubmitJsCodeCommand>();
            await javaScriptKernel.SubmitCodeAsync(@"
                interactive.registerCommandHandler({commandType: 'SubmitJsCodeCommand', handle: c => {
                    eval(c.command.code);
                }});");
        }


        //todo: don't remove all macro commands?
        private string RemoveMacroCommand(string code)
        {
            return string.Join(
                Environment.NewLine,
                code
                    .Split(Environment.NewLine)
                    .Where(line => !line.StartsWith("#!macro")));
        }

        private string FormatToastBody(Stopwatch stopwatch, CommandFailed? commandFailed)
        {
            StringBuilder body = new StringBuilder();
            body.Append("Time elaspsed : ");
            body.Append(stopwatch.Elapsed.TotalSeconds.ToString("0.00"));
            body.AppendLine(" secondes");

            if (commandFailed != null)
            {
                body.Append(commandFailed.Message);
                if (commandFailed.Exception != null)
                {
                    body.AppendLine();
                    body.Append(commandFailed.Exception.ToString());
                }
            }

            return body.ToString();
        }
    }
}