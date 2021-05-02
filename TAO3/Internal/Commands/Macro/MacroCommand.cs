using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Interop;
using WindowsHook;

namespace TAO3.Internal.Commands.Macro
{
    internal class MacroCommand : Command
    {
        public MacroCommand(IInteropOS interop)
            : base("#!macro", "Add a macro that run the code in the cell")
        {
            Add(new Argument<string>("shortcut", ""));

            Handler = CommandHandler.Create((string shortcut, KernelInvocationContext context) =>
            {
                Keys shortcutKeys = ShortcutParser.Parse(shortcut);
                string code = RemoveMacroCommand(((SubmitCode)context.Command).Code);
                Kernel rootKernel = context.HandlingKernel.ParentKernel;

                interop.KeyboardHook.RegisterOnKeyPressed(shortcutKeys, () =>
                {
                    rootKernel.SubmitCodeAsync(code);
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
