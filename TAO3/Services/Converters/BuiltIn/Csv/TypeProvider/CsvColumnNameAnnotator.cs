﻿using CsvHelper.Configuration.Attributes;
using TAO3.TypeProvider;

namespace TAO3.Converters.Csv;

internal class CsvColumnNameAnnotator : IPropertyAnnotator
{
    public void Annotate(ClassPropertySchema property, PropertyAnnotatorContext context)
    {
        context.Using(typeof(NameAttribute).Namespace!);
        context.StringBuilder.Append($@"[Name(""{property.FullName.Replace("\"", "\"\"")}"")]
    ");
    }
}
