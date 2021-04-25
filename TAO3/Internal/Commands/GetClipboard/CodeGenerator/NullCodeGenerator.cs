using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Internal.Commands.GetClipboard.CodeGenerator
{
    internal class NullCodeGenerator : ICodeGenerator
    {
        public async Task<string> GenerateSourceCodeAsync(GetClipboardOptions options)
        {
            string clipboardVariableName = "__cb";
            await options.CSharpKernel.SetVariableAsync(clipboardVariableName, options.Text, typeof(string));
            return $"string {options.Name} = {clipboardVariableName};";
        }
    }
}
