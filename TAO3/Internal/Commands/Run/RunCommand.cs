using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Cell;

namespace TAO3.Internal.Commands.Run
{
    internal class RunCommand : Command
    {
        public RunCommand(ICellService cellService) 
            : base("#!run")
        {
            Add(CreateRunCellCommand(cellService));
            Add(CreateRunVariableCommand());
            Add(CreateRunNotebookCommand());
        }

        private Command CreateRunCellCommand(ICellService cellService)
        {
            Command command = new Command("cell");

            cellService.Events.Subscribe(evnt =>
            {
                if (evnt is CellAddedEvent cellAddedEvent)
                {
                    NotebookCell cell = cellAddedEvent.Cell;
                    Command runCellCommand = new Command(cell.Name);

                    runCellCommand.Handler = CommandHandler.Create(async (KernelInvocationContext context) =>
                    {
                        await cell.Kernel.ParentKernel.SendAsync(new SubmitCode(cell.Code, targetKernelName: cell.Kernel.Name));
                    });

                    command.Add(runCellCommand);
                }
            });

            return command;
        }

        private Command CreateRunVariableCommand()
        {
            return new Command("variable");
        }

        private Command CreateRunNotebookCommand()
        {
            return new Command("notebook");
        }
    }
}
