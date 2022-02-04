using Microsoft.DotNet.Interactive.Commands;

namespace TAO3.Internal.Commands.Macro;

public class SubmitJsCodeCommand : KernelCommand
{
    public string Code { get; }

    public SubmitJsCodeCommand(string code) : base("javascript")
    {
        Code = code;
    }
}
