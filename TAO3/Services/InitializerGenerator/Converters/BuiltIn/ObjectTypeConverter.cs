using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TAO3.Internal.Types;

namespace TAO3.InitializerGenerator.Converters
{
    internal class ObjectTypeConverter : TypeConverter<object>
    {
        public override bool Convert(StringBuilder sb, object obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            ConstructorInfo[] constructorInfos = obj.GetType().GetConstructors();

            if (constructorInfos.Length != 1)
            {
                return false;
            }

            ConstructorInfo constructorInfo = constructorInfos[0];

            if (constructorInfo.GetParameters().Length == 0)
            {
                DTOStyleInitialization(sb, obj, generator, options);
                return true;
            }
            else
            {
                return RecordStyleInitialization(sb, obj, constructorInfo, generator, options);
            }
        }

        private void DTOStyleInitialization(StringBuilder sb, object obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            PropertyInfo[] propertyInfos = obj.GetType()
                .GetProperties()
                .Where(x => x.GetSetMethod() != null)
                .Where(x => x.GetSetMethod()!.IsPublic)
                .Where(x => x.GetGetMethod() != null)
                .ToArray();

            sb.Append("new ");
            sb.Append(obj.GetType().PrettyPrint());
            sb.Append("()");

            if (propertyInfos.Length == 0)
            {
                return;
            }

            sb.AppendLine();
            sb.Append(options.Indentation);
            sb.AppendLine("{");

            InitializerGeneratorOptions propertyOptions = options.Indent();

            foreach (PropertyInfo property in propertyInfos)
            {
                sb.Append(propertyOptions.Indentation);
                sb.Append(property.Name);
                sb.Append(" = ");
                generator.Generate(sb, property.GetValue(obj), propertyOptions);
                sb.AppendLine(",");
            }

            sb.Append(options.Indentation);
            sb.Append("}");
        }

        private bool RecordStyleInitialization(
            StringBuilder sb, 
            object obj, 
            ConstructorInfo constructorInfo,
            InitializerGeneratorService generator, 
            InitializerGeneratorOptions options)
        {
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties();

            Dictionary<ParameterInfo, PropertyInfo> binding = new Dictionary<ParameterInfo, PropertyInfo>();

            foreach (ParameterInfo parameter in constructorInfo.GetParameters())
            {
                PropertyInfo? property = propertyInfos
                    .Where(x => x.Name.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (property != null)
                {
                    binding[parameter] = property;
                }
                else if (!parameter.IsOptional)
                {
                    return false;
                }
            }

            sb.Append("new ");
            sb.Append(obj.GetType().PrettyPrint());
            sb.Append("(");

            InitializerGeneratorOptions argumentsOptions = options.Indent();

            bool isFirstProp = true;

            foreach (ParameterInfo parameter in constructorInfo.GetParameters())
            {
                PropertyInfo? propertyInfo = binding.GetValueOrDefault(parameter);

                if (propertyInfo != null)
                {
                    if (!isFirstProp)
                    {
                        sb.Append(",");
                    }

                    sb.AppendLine();
                    sb.Append(argumentsOptions.Indentation);

                    sb.Append(parameter.Name);
                    sb.Append(": ");
                    generator.Generate(sb, propertyInfo.GetValue(obj), argumentsOptions);

                    isFirstProp = false;
                }   
            }

            sb.Append(")");

            return true;
        }
    }
}
