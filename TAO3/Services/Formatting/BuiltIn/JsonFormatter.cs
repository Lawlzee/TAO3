using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace TAO3.Formatting;

public class JsonFormatter : IFormatter
{
    public string Format(string text)
    {
        using StringReader stringReader = new StringReader(text);
        using JsonTextReader reader = new JsonTextReader(stringReader);

        JToken token = JToken.ReadFrom(reader);
        return JsonConvert.SerializeObject(token, formatting: Newtonsoft.Json.Formatting.Indented);
    }
}
