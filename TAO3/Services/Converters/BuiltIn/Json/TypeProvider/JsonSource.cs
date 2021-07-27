using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.Json
{
    public class JsonSource
    {
        public string RootTypeName { get; }
        public string Json { get; }

        public JsonSource(string rootTypeName, string json)
        {
            RootTypeName = rootTypeName;
            Json = json;
        }
    }
}
