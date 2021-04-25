using Microsoft.DotNet.Interactive.CSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Internal.Commands.GetClipboard.CodeGenerator
{
    internal class JsonCodeGenerator : ICodeGenerator
    {
        public async Task<string> GenerateSourceCodeAsync(GetClipboardOptions options)
        {
            string clipboardVariableName = "__cb";
            await options.CSharpKernel.SetVariableAsync(clipboardVariableName, options.Text, typeof(string));

            string className = JsonClassGenerator.ToTitleCase(options.Name);
            string classDeclarations = JsonClassGenerator.GenerateClasses(options.Text, className);

            return $@"{classDeclarations}{className} {options.Name} = JsonConvert.DeserializeObject<{className}>({clipboardVariableName});";
        }
    }
}
