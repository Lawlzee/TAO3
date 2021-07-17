using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.CSharp;

namespace TAO3.TextSerializer.CSharp
{
    internal class ICSharpNodeTypeConverter : TypeConverter<ICSharpNode>
    {
        public override bool Convert(StringBuilder sb, ICSharpNode obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append(obj.Syntax.ToString());
            return true;
        }
    }
}
