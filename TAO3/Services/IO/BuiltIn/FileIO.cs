using System.CommandLine;
using System.IO;
using TAO3.Internal.Commands;

namespace TAO3.IO;

internal record FileOptions
{
    public string Path { get; init; } = null!;
    public Encoding? Encoding { get; init; }
}

internal class FileIO 
    : ITextSource<FileOptions>, 
    IDestination<FileOptions>,
    IConfigurableSource,
    IConfigurableDestination
{
    public string Name => "file";
    public IReadOnlyList<string> Aliases => Array.Empty<string>();

    public void Configure(Command command)
    {
        command.Add(CommandFactory.CreatePathArgument("path"));
        command.Add(CommandFactory.CreateEncodingOptions());
    }

    public Task<string> GetTextAsync(FileOptions options)
    {
        if (options.Encoding != null)
        {
            return File.ReadAllTextAsync(options.Path, options.Encoding);
        }

        return File.ReadAllTextAsync(options.Path);
    }

    public Task SetTextAsync(string text, FileOptions options)
    {
        if (options.Encoding != null)
        {
            return File.WriteAllTextAsync(options.Path, text, options.Encoding);
        }

        return File.WriteAllTextAsync(options.Path, text);
    }
}
