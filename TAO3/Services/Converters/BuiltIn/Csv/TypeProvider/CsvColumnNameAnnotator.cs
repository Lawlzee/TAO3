using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TypeProvider;

namespace TAO3.Converters.Csv
{
    internal class CsvColumnNameAnnotator : IPropertyAnnotator
    {
        public void Annotate(ClassPropertySchema property, PropertyAnnotatorContext context)
        {
            context.Using(typeof(NameAttribute).Namespace!);
            context.StringBuilder.Append($@"[Name(""{property.FullName.Replace("\"", "\"\"")}"")]
    ");
        }
    }
}
