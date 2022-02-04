using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TAO3.TypeProvider;

namespace TAO3.Converters.Json;

internal class JsonDomParser : IDomParser<JsonSource>
{
    public IDomType Parse(JsonSource input)
    {
        using StringReader stringReader = new StringReader(input.Json);
        using JsonTextReader reader = new JsonTextReader(stringReader);

        JToken token = JToken.ReadFrom(reader);

        return Visit(token, input.RootTypeName);
    }

    private IDomType Visit(JToken token, string currentName)
    {
        return token.Type switch
        {
            JTokenType.None => null,
            JTokenType.Object => Visit((JObject)token, currentName),
            JTokenType.Array => Visit((JArray)token, currentName),
            JTokenType.Constructor => null,
            JTokenType.Property => Visit((JProperty)token, currentName),
            JTokenType.Comment => null,
            JTokenType.Integer => new DomLiteral(typeof(int)),
            JTokenType.Float => new DomLiteral(typeof(double)),
            JTokenType.String => new DomLiteral(typeof(string)),
            JTokenType.Boolean => new DomLiteral(typeof(bool)),
            JTokenType.Null => new DomNullLiteral(),
            JTokenType.Undefined => new DomNullLiteral(),
            JTokenType.Date => new DomLiteral(typeof(DateTime)),
            JTokenType.Raw => new DomLiteral(typeof(string)),
            JTokenType.Bytes => new DomLiteral(typeof(byte[])),
            JTokenType.Guid => new DomLiteral(typeof(Guid)),
            JTokenType.Uri => new DomLiteral(typeof(string)),
            JTokenType.TimeSpan => new DomLiteral(typeof(TimeSpan)),
            _ => null
        } ?? throw new NotSupportedException(token.Type.ToString());
    }

    private IDomType Visit(JObject obj, string currentName)
    {
        return new DomClass(
            currentName,
            obj.Properties()
                .Select(Visit)
                .ToList());
    }

    private DomClassProperty Visit(JProperty property)
    {
        return new DomClassProperty(
            property.Name,
            Visit(property.Value, property.Name));
    }

    private DomCollection Visit(JArray array, string currentName)
    {
        return new DomCollection(array
            .Select(x => Visit(x, currentName))
            .ToList());
    }
}
