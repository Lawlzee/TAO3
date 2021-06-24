using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Notebook;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
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
            Add(CreateRunFileCommand());
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
                new FilePathArgument("path", "Path to a .dib, .ipynb, .csx or .cs file")
            };

            command.Handler = CommandHandler.Create(async (string path, KernelInvocationContext context) =>
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("File not found", path);
                }

                string extension = Path.GetExtension(path);

                if (extension.Equals(".dib", StringComparison.OrdinalIgnoreCase) 
                    || extension.Equals(".ipynb", StringComparison.OrdinalIgnoreCase))
                {
                    NotebookDocument notebook = Kernel.Current.ParentKernel.ParseNotebook(path, File.ReadAllBytes(path));
                    foreach (NotebookCell cell in notebook.Cells)
                    {
                        //todo: handle errors
                        await Kernel.Current.ParentKernel.SendAsync(new SubmitCode(cell.Contents, cell.Language));
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

        private class FilePathArgument : Argument<string>
        {
            public FilePathArgument(string name, string? description = null) 
                : base(name, description)
            {
            }

            public override IEnumerable<string?> GetSuggestions(ParseResult? parseResult = null, string? textToMatch = null)
            {
                if (textToMatch == null || parseResult == null)
                {
                    return base.GetSuggestions(parseResult, textToMatch);
                }

                string realTextToMatch = parseResult.CommandResult.Tokens.Count > 0
                    ? parseResult.CommandResult.Tokens[0].Value
                    : "";

                if (Path.IsPathFullyQualified(realTextToMatch))
                {
                    return GetRootedSuggestions(realTextToMatch);
                }

                return GetRelativeSuggestion(realTextToMatch);

                IEnumerable<string> GetRootedSuggestions(string textToMatch)
                {
                    if (File.Exists(textToMatch))
                    {
                        return new[] { AddQuoteIfNecessary(textToMatch) };
                    }

                    string? currentDirectory = Path.GetDirectoryName(textToMatch);
                    string fullyQualifiedCurrentDirectory = string.IsNullOrEmpty(currentDirectory)
                        ? Path.GetPathRoot(textToMatch)!
                        : currentDirectory!;

                    string? commandDirectory = Path.GetDirectoryName(textToMatch);
                    string directory = string.IsNullOrEmpty(commandDirectory)
                        ? fullyQualifiedCurrentDirectory
                        : commandDirectory;

                    string fileName = Path.GetFileName(textToMatch);

                    return Directory.GetDirectories(directory, fileName + "*")
                        .Select(x => x + "\\")
                        .Concat(Directory.GetFiles(directory, fileName + "*"))
                        .Select(AddQuoteIfNecessary);
                }

                IEnumerable<string> GetRelativeSuggestion(string textToMatch)
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    if (File.Exists(textToMatch))
                    {
                        return new[] { AddQuoteIfNecessary(Path.GetRelativePath(currentDirectory, textToMatch)) };
                    }

                    string? commandDirectory = Path.GetDirectoryName(textToMatch);
                    string directory = string.IsNullOrEmpty(commandDirectory)
                        ? currentDirectory
                        : commandDirectory;

                    string fileName = Path.GetFileName(textToMatch);

                    IEnumerable<string> suggestions = Directory.GetDirectories(directory, fileName + "*")
                        .Select(x => Path.GetRelativePath(currentDirectory, x) + "\\")
                        .Concat(Directory.GetFiles(directory, fileName + "*")
                            .Select(x => Path.GetRelativePath(currentDirectory, x)))
                        .Select(AddQuoteIfNecessary);

                    if (string.IsNullOrEmpty(commandDirectory))
                    {
                        return DriveInfo.GetDrives()
                            .Select(x => x.Name)
                            .Where(x => x.StartsWith(textToMatch, StringComparison.OrdinalIgnoreCase))
                            .Concat(suggestions)
                            .Select(AddQuoteIfNecessary);
                    }

                    return suggestions;
                }

                string AddQuoteIfNecessary(string path)
                {
                    if (path.Contains(" "))
                    {
                        return "\"" + path + "\"";
                    }

                    return path;
                }
            }
        }
    }
}
