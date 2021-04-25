using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Commands.GetClipboard.CodeGenerator;
using TAO3.Internal.Converters;
using TextCopy;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TAO3.Internal.Commands.GetClipboard
{
    internal class GetClipboardCommand : Command
    {
        public GetClipboardCommand() :
            base("#!cb", "Copy clipboard value")
        {
            Add(new Argument<DocumentType>("type"));
            Add(new Argument<string>("name"));
            Add(new Option<string>(new[] { "-s", "--separator" }, "Csv separator"));
            Add(new Option(new[] { "-v", "--verbose" }, "print generated class"));


            Handler = CommandHandler.Create(async (DocumentType type, string name, string separator, bool verbose, KernelInvocationContext context) =>
            {
                string text = await ClipboardService.GetTextAsync() ?? string.Empty;
                
                CSharpKernel cSharpKernel = (CSharpKernel)context.HandlingKernel.FindKernel("csharp");
                GetClipboardOptions options = new GetClipboardOptions(cSharpKernel, text, name, separator);

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
