using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TAO3.InitializerGenerator.Converters
{
    internal class AnonymousTypeConverter : ITypeConverter
    {
        public bool Convert(StringBuilder sb, object obj, Type objectType, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.AppendLine("new");
            sb.Append(options.Indentation);
            sb.Append("{");

            InitializerGeneratorOptions propOptions = options.Indent();

            PropertyInfo[] properties = obj.GetType().GetProperties();

            int i = 0;
            foreach (PropertyInfo propertyInfo in properties)
            {
                sb.AppendLine();
                sb.Append(propOptions.Indentation);

                sb.Append(propertyInfo.Name);
                sb.Append(" = ");

                generator.Generate(sb, propertyInfo.GetValue(obj), propOptions);

                i++;
                if (i != properties.Length)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();
            sb.Append(options.Indentation);
            sb.Append("}");

            return true;
        }

        public void Dispose()
        {

        }
    }
}
