using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;
using TAO3.Keyboard;
using TAO3.Toast;
using WindowsHook;

namespace TAO3.Internal.Commands.Macro
{
    internal class MacroCommand : Command
    {
        public MacroCommand(IKeyboardService keyboard, IToastService toast)
            : base("#!macro", "Add a macro that run the code in the cell")
        {
            Add(new Argument<string>("shortcut", "Ex. CTRL+SHIFT+1"));
            Add(new Option<string>(new[] { "-n", "--name" }, "Macro name"));
            Add(new Option(new[] { "-s", "--silent" }, "Disable the toast notifications"));

            Handler = CommandHandler.Create((string shortcut, string name, bool silent, KernelInvocationContext context) =>
            {
                Keys shortcutKeys = ShortcutParser.Parse(shortcut);
                SubmitCode originalCommand = (SubmitCode)context.Command;
                string code = RemoveMacroCommand(originalCommand.Code);
                CompositeKernel rootKernel = context.HandlingKernel.ParentKernel;

                keyboard.RegisterOnKeyPressed(shortcutKeys, () =>
                {
                    Stopwatch? stopwatch = null;
                    if (!silent)
                    {
                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                    }
                    
                    Task.Run(async () =>
                    {
                        context.Display(DateTime.Now);

                        SubmitCode submitCode = new SubmitCode(code, originalCommand.TargetKernelName, originalCommand.SubmissionType);

                        IDisposable disposable = null!;
                        CommandFailed? commandFailed = null;

                        disposable = rootKernel.KernelEvents.Subscribe(
                            onNext: e =>
                            {
                                KernelCommand rootCommand = e.Command.GetRootCommand();
                                if (rootCommand == submitCode && e is CommandFailed failed)
                                {
                                    commandFailed = failed;
                                }
                            });

                        await rootKernel.SendAsync(submitCode);
                        disposable.Dispose();

                        string title = (commandFailed, name) switch
                        {
                            (CommandFailed failed, _) => failed.Message,
                            (_, "") => $"{shortcut} ran successfully!",
                            (_, string macroName) => $"{macroName} ran successfully!",
                            _ => $"{shortcut} ran successfully!",
                        };

                        if (silent)
                        {
                            return;
                        }

                        stopwatch!.Stop();

                        StringBuilder body = new StringBuilder();
                        body.Append("Time elaspsed : ");
                        body.Append(stopwatch.Elapsed.TotalSeconds.ToString("0.00"));
                        body.AppendLine(" secondes");

                        if (commandFailed != null)
                        {
                            body.Append(commandFailed.Exception.ToString());
                        }

                        toast.Notify(title, body.ToString(), DateTimeOffset.Now.AddSeconds(1));
                    });
                });

                context.Display(@$"Macro was registered successfully. Use {shortcut} to run the macro. The result won't be visible in the notebook.", mimeType: null);
                context.Complete(context.Command);
            });
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
    }
}
