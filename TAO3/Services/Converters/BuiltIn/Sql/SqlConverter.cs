using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.Sql;
using TAO3.TypeProvider;

namespace TAO3.Converters.Sql
{
    public class SqlConverter : IConverter
    {
        private readonly ITypeProvider<string> _typeProvider;
        public ISqlObjectSerializer Serializer { get; }

        public string Format => "sql";
        public string DefaultType => "dynamic";

        public SqlConverter(ITypeProvider<string> typeProvider, ISqlObjectSerializer serializer)
        {
            _typeProvider = typeProvider;
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
