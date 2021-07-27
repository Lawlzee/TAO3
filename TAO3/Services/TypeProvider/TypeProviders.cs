using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.Csv;
using TAO3.Converters.Json;
using TAO3.Converters.Sql;

namespace TAO3.TypeProvider
{
    public interface ITypeProviders : IDisposable
    {
        ICSharpSchemaSerializer Serializer { get; }
        ITypeProvider<string> Sql { get; }
        ITypeProvider<JsonSource> Json { get; }
        ITypeProvider<CsvSource> Csv { get; }
    }

    public class TypeProviders : ITypeProviders
    {
        public ICSharpSchemaSerializer Serializer { get; }
        public ITypeProvider<string> Sql { get; }
        public ITypeProvider<JsonSource> Json { get; }
        public ITypeProvider<CsvSource> Csv { get; }

        public TypeProviders(ICSharpSchemaSerializer serializer, ITypeProvider<string> sql, ITypeProvider<JsonSource> json, ITypeProvider<CsvSource> csv)
        {
            Serializer = serializer;
            Sql = sql;
            Json = json;
            Csv = csv;
        }

        public void Dispose()
        {
            
        }
    }
}
