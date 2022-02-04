using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace TAO3.IO;

internal class CellSource : ITextSource
{
    public string Name => "cell";

    public IReadOnlyList<string> Aliases => Array.Empty<string>();

    public Task<string> GetTextAsync()
    {
        string code = ((SubmitCode)KernelInvocationContext.Current.Command).Code;
        return Task.Run(() => string.Join(
            Environment.NewLine, 
            code.Split(Environment.NewLine, StringSplitOptions.None)
                .Skip(1)));
    }
}
