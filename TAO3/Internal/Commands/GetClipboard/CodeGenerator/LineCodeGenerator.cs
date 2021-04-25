using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace TAO3.Internal.Commands.GetClipboard.CodeGenerator
{
    internal class LineCodeGenerator : ICodeGenerator
    {
        public async Task<string> GenerateSourceCodeAsync(GetClipboardOptions options)
        {
            string clipboardVariableName = "__cb";
            await options.CSharpKernel.SetVariableAsync(clipboardVariableName, options.Text, typeof(string));

            string separator = options.Separator != string.Empty
                ? options.Separator.Replace("\"", "\"\"")
                : @"\r\n|\r|\n";

            return @$"using System.Text.RegularExpressions;

string[] {options.Name} = Regex.Split({clipboardVariableName}, @""{separator}"");";
        }
    }
}
