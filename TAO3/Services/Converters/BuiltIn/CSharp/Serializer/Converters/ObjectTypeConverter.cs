using System.Reflection;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class ObjectTypeConverter : TypeConverter<object, CSharpSerializerSettings>
{
    public override bool Convert(object obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        ConstructorInfo[] constructorInfos = obj.GetType().GetConstructors();

        if (constructorInfos.Length != 1)
        {
            return false;
        }

        ConstructorInfo constructorInfo = constructorInfos[0];

        if (constructorInfo.GetParameters().Length == 0)
        {
            DTOStyleInitialization(obj, context);
            return true;
        }
        else
        {
            return RecordStyleInitialization(obj, constructorInfo, context);
        }
    }

    private void DTOStyleInitialization(object obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        PropertyInfo[] propertyInfos = obj.GetType()
            .GetProperties()
            .Where(x => x.GetSetMethod() != null)
            .Where(x => x.GetSetMethod()!.IsPublic)
            .Where(x => x.GetGetMethod() != null)
            .ToArray();

        context.Append("new ");
        context.Append(obj.GetType().PrettyPrint());
        context.Append("()");

        if (propertyInfos.Length == 0)
        {
            return;
        }

        context.AppendLine();
        context.AppendIndentation();
        context.AppendLine("{");

        ObjectSerializerContext<CSharpSerializerSettings> propertyContext = context.Indent();

        foreach (PropertyInfo property in propertyInfos)
        {
            propertyContext.AppendIndentation();
            propertyContext.Append(property.Name);
            propertyContext.Append(" = ");
            propertyContext.Serialize(property.GetValue(obj));
            propertyContext.AppendLine(",");
        }

        context.AppendIndentation();
        context.Append("}");
    }

    private bool RecordStyleInitialization(
        object obj, 
        ConstructorInfo constructorInfo,
        ObjectSerializerContext<CSharpSerializerSettings> context)
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

        context.Append("new ");
        context.Append(obj.GetType().PrettyPrint());
        context.Append("(");

        ObjectSerializerContext<CSharpSerializerSettings> argumentsContext = context.Indent();

        bool isFirstProp = true;

        foreach (ParameterInfo parameter in constructorInfo.GetParameters())
        {
            PropertyInfo? propertyInfo = binding.GetValueOrDefault(parameter);

            if (propertyInfo != null)
            {
                if (!isFirstProp)
                {
                    argumentsContext.Append(",");
                }

                argumentsContext.AppendLine();
                argumentsContext.AppendIndentation();

                argumentsContext.Append(parameter.Name);
                argumentsContext.Append(": ");
                argumentsContext.Serialize(propertyInfo.GetValue(obj));

                isFirstProp = false;
            }   
        }

        context.Append(")");

        return true;
    }
}
