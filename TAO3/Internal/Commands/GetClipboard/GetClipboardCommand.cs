using Microsoft.CodeAnalysis.CSharp;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAO3.Internal.Commands.GetClipboard.CodeGenerator;
using TAO3.Internal.Converters;
using TAO3.Internal.Interop;

namespace TAO3.Internal.Commands.GetClipboard
{
    internal class GetClipboardCommand : Command
    {
        public GetClipboardCommand(IInteropOS interop) :
            base("#!cb", "Copy clipboard value")
        {
            Add(new Argument<DocumentType>("type", "The document type of the clipboard"));
            Add(new Argument<string>("name", "The name of the variable that will contain the deserialized clipboard content"));
            Add(new Option<string>(new[] { "-s", "--separator" }, "Separator for CSV format and line separator regex for line format"));
            Add(new Option(new[] { "-v", "--verbose" }, "Print debugging information"));
            Add(new Option(new[] { "-d", "--dynamic" }, "Disable the class definition generation and use dynamic object instead"));

            Handler = CommandHandler.Create(async (DocumentType type, string name, string separator, bool verbose, bool dynamic, KernelInvocationContext context) =>
            {
                string text = await interop.Clipboard.GetTextAsync() ?? string.Empty;
                
                CSharpKernel cSharpKernel = (CSharpKernel)context.HandlingKernel.FindKernel("csharp");
                GetClipboardOptions options = new GetClipboardOptions(cSharpKernel, text, name, Regex.Unescape(separator), dynamic);

                string code = await CodeGeneratorFactory.Create(type).GenerateSourceCodeAsync(options);

                if (verbose)
                {
                    context.Display(code, null);
                }

                await cSharpKernel.SubmitCodeAsync(code);
            });
        }
    }
}
