using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.CSharp
{
    internal class SyntaxNodeTypeConverter : TypeConverter<SyntaxNode>
    {
        public override bool Convert(StringBuilder sb, SyntaxNode obj, ObjectSerializer generator, ObjectSerializerOptions options)
        {
            sb.Append(obj.ToString());
            return true;
        }
    }
}
