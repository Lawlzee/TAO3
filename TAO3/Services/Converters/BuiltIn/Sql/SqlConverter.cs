using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.SQL;

namespace TAO3.Converters
{
    public class SqlConverter : IConverter
    {
        public ISqlObjectSerializer Serializer { get; }

        public string Format => "sql";
        public string DefaultType => "dynamic";

        public SqlConverter(ISqlObjectSerializer serializer)
        {
            Serializer = serializer;
        }

        public object? Deserialize<T>(string text)
        {
            throw new NotImplementedException();
        }

        public string Serialize(object? value)
        {
            return Serializer.Serialize(value);
        }
    }
}
