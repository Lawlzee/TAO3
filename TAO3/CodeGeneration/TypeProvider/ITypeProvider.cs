using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public interface ITypeProvider<TInput>
    {
        SchemaSerialization ProvideTypes(TInput input);
    }

    public class TypeProvider<TInput> : ITypeProvider<TInput>
    {
        private readonly string _format;
        private readonly IDomParser<TInput> _domParser;
        private readonly IDomSchematizer _schematizer;
        private readonly IDomSchemaSerializer _serializer;

        public TypeProvider(
            string format, 
            IDomParser<TInput> domParser, 
            IDomSchematizer schematizer, 
            IDomSchemaSerializer serializer)
        {
            _format = format;
            _domParser = domParser;
            _schematizer = schematizer;
            _serializer = serializer;
        }

        public SchemaSerialization ProvideTypes(TInput input)
        {
            IDomType dom = _domParser.Parse(input);
            DomSchema schema = _schematizer.Schematize(dom, _format);
            SchemaSerialization serialization = _serializer.Serialize(schema);
            return serialization;
        }
    }
}
