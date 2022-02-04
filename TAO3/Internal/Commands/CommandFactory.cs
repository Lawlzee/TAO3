using Microsoft.DotNet.Interactive.CSharp;
using System.CommandLine;
using System.CommandLine.Completions;
using System.IO;

namespace TAO3.Internal.Commands;

internal static class CommandFactory
{
    public static Option<T?> CreateVariableOption<T>(string[] aliases, string description, CSharpKernel cSharpKernel)
    {
        Option<T?> option = new Option<T?>(aliases, description: description, parseArgument: result =>
        {
            if (result.Tokens.Count == 0)
            {
                return default;
            }

            string variableName = result.Tokens[0].Value;
            if (cSharpKernel.TryGetValue(variableName, out object variableInstance))
            {
                return (T)variableInstance;
            }

            result.ErrorMessage = $"The variable '{variableName}' was not found in the C# Kernel";
            return default;
        });

        option.AddCompletions(context =>
        {
            return cSharpKernel
                .GetValueInfos()
                .Select(x => x.Name)
                .Where(x => context.WordToComplete?.Contains(x) ?? true);
        });

        return option;
    }

    public static Option<TSettings?> CreateSettingsOption<TSettings>(CSharpKernel cSharpKernel)
    {
        return CreateVariableOption<TSettings>(new[] { "--settings" }, description: $"Converter settings of type '{typeof(TSettings).FullName}'", cSharpKernel);
    }

    public static Option<Encoding?> CreateEncodingOptions()
    {
        Option<Encoding?> encodingOptions = new Option<Encoding?>("--encoding", result =>
        {
            if (result.Tokens.Count == 0)
            {
                return null;
            }

            string encodingName = result.Tokens[0].Value;
            EncodingInfo? encodingInfo = Encoding.GetEncodings()
                .FirstOrDefault(x => x.Name == encodingName);

            if (encodingInfo == null)
            {
                result.ErrorMessage = $"The encoding '{encodingName}' is invalid";
                return null;
            }

            return encodingInfo.GetEncoding();
        });


        encodingOptions.AddCompletions(Encoding
            .GetEncodings()
            .Select(x => x.Name)
            .ToArray());

        return encodingOptions;
    }

    public static Argument<string> CreatePathArgument(string name, string? description = null)
    {
        return new FilePathArgument(name, description);
    }

    private class FilePathArgument : Argument<string>
    {
        public FilePathArgument(string name, string? description = null)
            : base(name, description)
        {
        }

        public override IEnumerable<CompletionItem> GetCompletions(CompletionContext context)
        {
            if (context.WordToComplete == null || context.ParseResult.CommandResult.Command.Arguments.Count == 0)
            {
                return base.GetCompletions(context);
            }

            int tokenIndex = context.ParseResult.CommandResult.Command.Arguments
                .Select((x, i) => new
                {
                    Arg = x,
                    Index = i
                })
                .Where(x => x.Arg.Name == Name)
                .Select(x => x.Index)
                .First();

            if (context.ParseResult.CommandResult.Tokens.Count <= tokenIndex)
            {
                return Array.Empty<CompletionItem>();
            }

            string realTextToMatch = context.ParseResult.CommandResult.Tokens[tokenIndex].Value;

            if (Path.IsPathFullyQualified(realTextToMatch))
            {
                return GetRootedSuggestions(realTextToMatch);
            }

            return GetRelativeSuggestion(realTextToMatch);

            IEnumerable<CompletionItem> GetRootedSuggestions(string textToMatch)
            {
                if (File.Exists(textToMatch))
                {
                    return new[] { new CompletionItem(AddQuoteIfNecessary(textToMatch)) };
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
                    .Select(AddQuoteIfNecessary)
                    .Select(x => new CompletionItem(x));
            }

            IEnumerable<CompletionItem> GetRelativeSuggestion(string textToMatch)
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                if (File.Exists(textToMatch))
                {
                    return new[] { new CompletionItem(AddQuoteIfNecessary(Path.GetRelativePath(currentDirectory, textToMatch))) };
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
                        .Select(AddQuoteIfNecessary)
                        .Select(x => new CompletionItem(x));
                }

                return suggestions
                    .Select(x => new CompletionItem(x));
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
