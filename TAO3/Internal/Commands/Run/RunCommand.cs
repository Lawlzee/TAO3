using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Documents;
using Microsoft.DotNet.Interactive.Documents.ParserServer;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using TAO3.Cell;
using TAO3.Internal.Extensions;
using TAO3.Internal.Utils;
using TAO3NotebookCell = TAO3.Cell.NotebookCell;

namespace TAO3.Internal.Commands.Run;

internal class RunCommand : Command
{
    public RunCommand(ICellService cellService) 
        : base("#!run")
    {
        Add(CreateRunCellCommand(cellService));
        Add(CreateRunVariableCommand());
        Add(CreateRunFileCommand());
    }

    private Command CreateRunCellCommand(ICellService cellService)
    {
        Command command = new Command("cell");

        Dictionary<string, Command> commandByCellName = new();

        cellService.Events.RegisterChildCommand<ICellEvent, CellAddedEvent, CellRemovedEvent>(
            this,
            x => x.Cell.Name,
            evnt =>
            {
                TAO3NotebookCell cell = evnt.Cell;
                Command runCellCommand = new Command(evnt.Cell.Name);

                runCellCommand.Handler = CommandHandler.Create(async () =>
                {
                    await cell.Kernel.ParentKernel.SendAsync(new SubmitCode(cell.Code, targetKernelName: cell.Kernel.Name));
                });

                return runCellCommand;
            });

        return command;
    }

    private Command CreateRunVariableCommand()
    {
        Command command = new Command("variable")
        {
            new Argument<string>("name", "name of the variable")
        };

        return command;
    }

    private Command CreateRunFileCommand()
    {
        Command command = new Command("file")
        {
            CommandFactory.CreatePathArgument("path", "Path to a .dib, .ipynb, .csx or .cs file")
        };

        command.Handler = CommandHandler.Create(async (string path, KernelInvocationContext context) =>
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found", path);
            }

            string extension = Path.GetExtension(path);

            bool isDib = extension.Equals(".dib", StringComparison.OrdinalIgnoreCase);
            bool isIpynb = extension.Equals(".ipynb", StringComparison.OrdinalIgnoreCase);
            if (isDib || isIpynb)
            {
                NotebookParseRequest request = new NotebookParseRequest(
                    id: Guid.NewGuid().ToString(),
                    serializationType: isDib ? DocumentSerializationType.Dib : DocumentSerializationType.Ipynb,
                    defaultLanguage: context.HandlingKernel.Name,
                    rawData: await File.ReadAllBytesAsync(path));

                NotebookParseResponse response = (NotebookParseResponse)NotebookParserServer.HandleRequest(request);

                foreach (InteractiveDocumentElement cell in response.Document.Elements)
                {
                    //todo: handle errors
                    await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(cell.Contents, cell.KernelName));
                }
            }
            else if (extension.Equals(".csx", StringComparison.OrdinalIgnoreCase))
            {
                string code = File.ReadAllText(path);
                await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(code, "C#"));
            }
            else if (extension.Equals(".fs", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".fsi", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".fsx", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".fsscript", StringComparison.OrdinalIgnoreCase))
            {
                string code = File.ReadAllText(path);
                await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(code, "F#"));
            }
            else if (extension.Equals(".js", StringComparison.OrdinalIgnoreCase))
            {
                string code = File.ReadAllText(path);
                await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(code, "javascript"));
            }
            else if (extension.Equals(".html", StringComparison.OrdinalIgnoreCase))
            {
                string code = File.ReadAllText(path);
                await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(code, "html"));
            }
            else if (extension.Equals(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                string code = File.ReadAllText(path);
                await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(code, "powershell"));
            }
            else
            {
                string rawCode = File.ReadAllText(path);
                string preprocessedCode = NamespaceRemover.RemoveNamespaces(rawCode);
                await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(preprocessedCode, "C#"));
            }
            
        });

        return command;
    }
}
