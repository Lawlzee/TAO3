using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TAO3.Internal.Converters;

namespace TAO3.Internal.Commands.GetClipboard.CodeGenerator
{
    internal class XmlCodeGenerator : ICodeGenerator
    {
        public async Task<string> GenerateSourceCodeAsync(GetClipboardOptions options)
        {
            XDocument document = XDocument.Parse(options.Text);
            XElement rootElement = document.Root!;

            string jsonInput = JsonConvert.SerializeXNode(rootElement, Formatting.None, omitRootObject: true);

            return await CodeGeneratorFactory.Create(DocumentType.Json)
                .GenerateSourceCodeAsync(options with
                {
                    Text = jsonInput
                });
        }
    }
}
