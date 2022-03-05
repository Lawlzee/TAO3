using System.CommandLine;
using Microsoft.DotNet.Interactive.CSharp;

namespace TAO3.IO;

internal record SourceVariableOptions
{
    public string SourceVariableName { get; init; } = null!;
}

internal record DestinationVariableOptions
{
    public string DestinationVariableName { get; init; } = null!;
}

internal class VariableIO :
    ITextSource<SourceVariableOptions>,
    IDestination<DestinationVariableOptions>,
    IConfigurableSource,
    IConfigurableDestination
{
    public string Name { get; } = "variable";
    public IReadOnlyList<string> Aliases { get; } = new[] { "var" };

    private readonly CSharpKernel _cSharpKernel;

    public VariableIO(CSharpKernel cSharpKernel)
    {
        _cSharpKernel = cSharpKernel;
    }

    void IConfigurableSource.Configure(Command command)
    {
        command.Add(new Argument("sourceVariableName", "Name of the variable to use as a source"));
    }

    void IConfigurableDestination.Configure(Command command)
    {
        command.Add(new Argument("destinationVariableName", "Name of the variable to use as a destination"));
    }

    public Task<string> GetTextAsync(SourceVariableOptions options)
    {
        string text = _cSharpKernel.TryGetValue(options.SourceVariableName, out object value)
            ? value?.ToString() ?? ""
            : throw new ArgumentException($"No variable named {options.SourceVariableName} is declared in the C# kernel", "SourceVariableName");

        return Task.FromResult(text);
    }

    public async Task SetTextAsync(string text, DestinationVariableOptions options)
    {
        await _cSharpKernel.SetValueAsync(options.DestinationVariableName, text, typeof(string));
    }
}
