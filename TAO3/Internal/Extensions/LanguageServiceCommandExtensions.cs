using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Parsing;
using System.Reflection;

namespace TAO3.Internal.Extensions;

internal static class LanguageServiceCommandExtensions
{
    private static readonly PropertyInfo _languageNodeProperty = typeof(LanguageServiceCommand)
        .GetProperty("LanguageNode", BindingFlags.NonPublic | BindingFlags.Instance)!;

    public static KernelNameDirectiveNode GetKernelNameDirectiveNode(this LanguageServiceCommand command)
    {
        LanguageNode languageNode = (LanguageNode)_languageNodeProperty.GetValue(command)!;
        List<SyntaxNode> syntaxNodes = languageNode.Parent!.ChildNodes.ToList();
        int index = syntaxNodes.IndexOf(languageNode);
        return (KernelNameDirectiveNode)syntaxNodes[index - 1];
    }
}
