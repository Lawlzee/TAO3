using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Commands.GetClipboard.CodeGenerator
{
    internal record GetClipboardOptions(
        CSharpKernel CSharpKernel,
        string Text,
        string Name,
        string Separator,
        bool Dynamic);

    internal record GenerationResult(
        string ClassDefinition,
        string ClassName);

    internal interface ICodeGenerator
    {
        Task<string> GenerateSourceCodeAsync(GetClipboardOptions options);
    }
}
