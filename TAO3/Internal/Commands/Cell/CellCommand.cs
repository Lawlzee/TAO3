using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Cell;
using TAO3.Internal.Extensions;

namespace TAO3.Internal.Commands.Cell
{
    internal class CellCommand : Command
    {
        public CellCommand(ICellService cellService)
            : base("#!cell", "Add a named cell")
        {
            Add(new Argument<string>("name", "Name of the cell"));

            Handler = CommandHandler.Create((string name, KernelInvocationContext context) =>
            {
                KernelCommand command = context.Command.GetRootCommand();

                if (command is SubmitCode submitCode && NotebookCell.ContainsCellDirective(name, submitCode.Code))
                {
                    cellService.AddOrUpdateCell(name, submitCode.Code, context.HandlingKernel);
                }
            });
        }
    }
}
