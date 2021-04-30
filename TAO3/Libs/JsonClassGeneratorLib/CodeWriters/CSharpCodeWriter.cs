using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    internal class CSharpCodeWriter
    {
        internal string GetTypeName(JsonType type)
        {
            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "object";
                case JsonTypeEnum.Array: return "List<" + GetTypeName(type.InternalType) + ">";
                case JsonTypeEnum.Dictionary: return "Dictionary<string, " + GetTypeName(type.InternalType) + ">";
                case JsonTypeEnum.Boolean: return "bool";
                case JsonTypeEnum.Float: return "double";
                case JsonTypeEnum.Integer: return "int";
                case JsonTypeEnum.Long: return "long";
                case JsonTypeEnum.Date: return "DateTime";
                case JsonTypeEnum.NonConstrained: return "object";
                case JsonTypeEnum.NullableBoolean: return "bool?";
                case JsonTypeEnum.NullableFloat: return "double?";
                case JsonTypeEnum.NullableInteger: return "int?";
                case JsonTypeEnum.NullableLong: return "long?";
                case JsonTypeEnum.NullableDate: return "DateTime?";
                case JsonTypeEnum.NullableSomething: return "object";
                case JsonTypeEnum.Object: return type.AssignedName;
                case JsonTypeEnum.String: return "string";
                default: throw new NotSupportedException("Unsupported json type");
            }
        }

        internal void WriteFileStart(TextWriter sw)
        {
            sw.WriteLine("using System;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using CsvHelper.Configuration.Attributes;");
            sw.WriteLine("using Newtonsoft.Json;");
            sw.WriteLine("using Newtonsoft.Json.Linq;");
            sw.WriteLine();
        }

        internal void WriteFileEnd(TextWriter sw)
        {

        }

        internal void WriteClass(TextWriter sw, JsonType type)
        {
            sw.WriteLine("public class {0}", type.AssignedName);
            sw.Write("{");

            int i = 0;
            foreach (FieldInfo field in type.Fields)
            {
                sw.WriteLine();
                sw.WriteLine("    [JsonProperty(\"{0}\")]", field.JsonMemberName);
                sw.WriteLine("    [Index({0})]", i);
                sw.WriteLine("    public {0} {1} {{ get; set; }}", GetTypeName(field.Type), field.MemberName);
                i++;
            }

            sw.WriteLine("}");

            sw.WriteLine();
        }
    }
}
