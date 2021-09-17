using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Extensions
{
    internal static class RequestDiagnosticsExtensions
    {
        private static readonly PropertyInfo _languageNodeProperty = typeof(RequestDiagnostics)
            .GetProperty("LanguageNode", BindingFlags.NonPublic | BindingFlags.Instance)!;

        public static KernelNameDirectiveNode GetKernelNameDirectiveNode(this RequestDiagnostics command)
        {
            LanguageNode languageNode = (LanguageNode)_languageNodeProperty.GetValue(command)!;
            List<SyntaxNode> syntaxNodes = languageNode.Parent!.ChildNodes.ToList();
            int index = syntaxNodes.IndexOf(languageNode);
            return (KernelNameDirectiveNode)syntaxNodes[index - 1];
        }
    }
}
