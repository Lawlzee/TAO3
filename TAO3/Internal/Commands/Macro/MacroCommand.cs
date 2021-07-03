using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;
using TAO3.Keyboard;
using TAO3.Toast;
using WindowsHook;

namespace TAO3.Internal.Commands.Macro
{
    internal class MacroCommand : Command
    {
        private MacroCommand(IKeyboardService keyboard, IToastService toast)
            : base("#!macro", "Add a macro that run the code in the cell")
        {
            Add(new Argument<string>("shortcut", "Ex. CTRL+SHIFT+1"));
            Add(new Option<string>(new[] { "-n", "--name" }, "Macro name"));
            Add(new Option(new[] { "-s", "--silent" }, "Disable the toast notifications"));

            Handler = CommandHandler.Create(async (string shortcut, string name, bool silent, KernelInvocationContext context) =>
            {
                Keys shortcutKeys = ShortcutParser.Parse(shortcut);
                SubmitCode originalCommand = (SubmitCode)context.Command;
                string code = RemoveMacroCommand(originalCommand.Code);
                CompositeKernel rootKernel = context.HandlingKernel.ParentKernel;

                Kernel javascriptKernel = rootKernel.FindKernel("javascript");
                string cellId = $"__internal_cell_{Guid.NewGuid():N}";

                keyboard.RegisterOnKeyPressed(shortcutKeys, () =>
                {
                    Stopwatch? stopwatch = null;
                    if (!silent)
                    {
                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                    }

                    _ = Task.Run(async () =>
                    {
                        context.Display(DateTime.Now);

                        SubmitCode submitCode = new SubmitCode(code, originalCommand.TargetKernelName, originalCommand.SubmissionType);

                        (CommandFailed? commandFailed, ReturnValueProduced? returnValueProduced) = await RunCellAsync(rootKernel, submitCode);

                        string title = (commandFailed, name) switch
                        {
                            (CommandFailed failed, _) => failed.Message,
                            (_, "") => $"{shortcut} ran successfully!",
                            (_, string macroName) => $"{macroName} ran successfully!",
                            _ => $"{shortcut} ran successfully!",
                        };
                        await PrintCellResultAsync(silent, javascriptKernel, cellId, commandFailed, returnValueProduced, title);

                        if (silent)
                        {
                            return;
                        }

                        stopwatch!.Stop();
                        string body = FormatToastBody(stopwatch, commandFailed);

                        toast.Notify(title, body, DateTimeOffset.Now.AddSeconds(1));
                    });
                });

                Kernel htmlKernel = rootKernel.FindKernel("html");
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
        }

        public static async Task<MacroCommand> CreateAsync(IKeyboardService keyboard, IToastService toast)
        {
            await RegisterSendJavascriptCodeCommandAsync();
            return new MacroCommand(keyboard, toast);
        }

        private static async Task RegisterSendJavascriptCodeCommandAsync()
        {
            Kernel jsKernel = Kernel.Root.FindKernel("javascript");
            jsKernel.RegisterCommandType<SubmitJsCodeCommand>();
            await jsKernel.SubmitCodeAsync(@"
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

        private async Task<(CommandFailed?, ReturnValueProduced?)> RunCellAsync(CompositeKernel rootKernel, SubmitCode submitCode)
        {
            IDisposable? disposable = null;
            CommandFailed? commandFailed = null;
            ReturnValueProduced? returnValueProduced = null;

            try
            {
                disposable = rootKernel.KernelEvents.Subscribe(
                onNext: e =>
                {
                    KernelCommand rootCommand = e.Command.GetRootCommand();
                    if (rootCommand == submitCode)
                    {
                        if (e is CommandFailed failed)
                        {
                            commandFailed = failed;
                        }

                        if (e is ReturnValueProduced valueProduced)
                        {
                            returnValueProduced = valueProduced;
                        }
                    }
                });

                await rootKernel.SendAsync(submitCode);
                return (commandFailed, returnValueProduced);
            }
            finally
            {
                disposable?.Dispose();
            }
        }


        private async Task PrintCellResultAsync(
            bool silent, 
            Kernel javascriptKernel, 
            string cellId, 
            CommandFailed? commandFailed, 
            ReturnValueProduced? returnValueProduced, 
            string title)
        {
            string[] body = (returnValueProduced != null, commandFailed != null, silent) switch
            {
                (true, _, _) => returnValueProduced!.FormattedValues.Select(x => x.Value).ToArray(),
                (_, true, _) => FormatCommandFailedBody(),
                (_, _, true) => new[] { title },
                _ => Array.Empty<string>()
            };

            string json = JsonConvert.SerializeObject(body);
            await javascriptKernel.SendAsync(new SubmitJsCodeCommand($@"{cellId}_Print({json})"));

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