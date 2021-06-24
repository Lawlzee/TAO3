using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Notebook;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Cell;
using NotebookCell = Microsoft.DotNet.Interactive.Notebook.NotebookCell;
using TAO3NotebookCell = TAO3.Cell.NotebookCell;

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
                    TAO3NotebookCell cell = cellAddedEvent.Cell;
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
            Command command = new Command("notebook")
            {
                new Argument<string>("path", "Path to a .dib or .ipynb file")
            };

            command.Handler = CommandHandler.Create(async (string path, KernelInvocationContext context) =>
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("File not found", path);
                }

                NotebookDocument notebook = Kernel.Current.ParentKernel.ParseNotebook(path, File.ReadAllBytes(path));
                foreach (NotebookCell cell in notebook.Cells)
                {
                    //todo: handle errors
                    await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(cell.Contents, cell.Language));
                }
            });

            return command;
        }
    }
}
