using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Converters;

namespace TAO3.Internal.Commands.GetClipboard.CodeGenerator
{
    internal class CodeGeneratorFactory
    {
        internal static ICodeGenerator Create(DocumentType documentType)
        {
            switch (documentType)
            {
                case DocumentType.Json:
                    return new JsonCodeGenerator();
                case DocumentType.Xml:
                    return new XmlCodeGenerator();
                case DocumentType.Line:
                    return new LineCodeGenerator();
                case DocumentType.Csv:
                    return new CsvCodeGenerator(hasHeader: false);
                case DocumentType.Csvh:
                    return new CsvCodeGenerator(hasHeader: true);
                default:
                    return new NullCodeGenerator();
            }
        }
    }
}
