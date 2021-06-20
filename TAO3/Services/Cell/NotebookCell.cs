using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;

namespace TAO3.Cell
{
    public class NotebookCell : IDisposable
    {
        public Kernel Kernel { get; internal set;}
        private IDisposable? _disposable;

        public string Name { get; }
        public string Code { get; set; }
        public object? Result { get; private set; }

        internal NotebookCell(string name, string code, Kernel kernel)
        {
            Name = name;
            Code = code;
            Kernel = kernel;
        }

        internal static NotebookCell Create(string name, string code, Kernel kernel)
        {
            NotebookCell notebookCell = new NotebookCell(name, code, kernel);

            object? htmlDisplayValue = null;

            notebookCell._disposable = kernel.ParentKernel.KernelEvents.Subscribe(
                onNext: e =>
                {
                    KernelCommand rootCommand = e.Command.GetRootCommand();
                    if (rootCommand is SubmitCode submitCode && ContainsCellDirective(name, submitCode.Code))
                    {
                        notebookCell.Code = submitCode.Code;

                        if (e is CommandFailed failed)
                        {
                            notebookCell.Result = null;
                        }
                        else if (e is ReturnValueProduced valueProduced)
                        {
                            notebookCell.Result = valueProduced.Value;
                        }
                        else if (notebookCell.Kernel is HtmlKernel)
                        {
                            if (e is DisplayedValueProduced displayedValue)
                            {
                                htmlDisplayValue = displayedValue.Value;
                            }
                            else if (e is CommandSucceeded)
                            {
                                notebookCell.Result = htmlDisplayValue;
                            }
                        }
                    }
                });

            return notebookCell;
        }

        internal static bool ContainsCellDirective(string name, string submittedCode)
        {
            string regex = $@"^#!cell\s+{Regex.Escape(name)}\s*$";
            return Regex.IsMatch(submittedCode, regex, RegexOptions.Multiline);
        }

        public async Task RunAsync()
        {
            await Kernel.ScheduleSubmitCodeAsync(Code);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
