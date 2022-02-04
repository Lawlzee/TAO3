using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Reflection;
using TAO3.Internal.Types;

namespace TAO3.Converters.Xml;

//https://stackoverflow.com/questions/11320968/can-newtonsoft-json-net-skip-serializing-empty-lists
internal class IgnoreEmptyCollectionContractResolver : DefaultContractResolver
{
    public static readonly IgnoreEmptyCollectionContractResolver Instance = new IgnoreEmptyCollectionContractResolver();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyType != typeof(string))
        {
            if (property.PropertyType!.GetInterface(nameof(IEnumerable)) != null)
            {
                property.ShouldSerialize = instance =>
                {
                    if (instance == null)
                    {
                        return false;
                    }

                    object? value = member.GetValue(instance);
                    return value != null && ((IEnumerable)value).GetEnumerator().MoveNext();
                };
            }
                
        }
        return property;
    }
}
