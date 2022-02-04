using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using TAO3.Cell;
using TAO3.Internal.Extensions;

namespace TAO3.Internal.Commands.Cell;

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
