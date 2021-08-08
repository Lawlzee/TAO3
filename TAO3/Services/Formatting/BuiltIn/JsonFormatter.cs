using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Formatting
{
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
}
