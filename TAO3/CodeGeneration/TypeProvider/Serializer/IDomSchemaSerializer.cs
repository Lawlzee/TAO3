using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public interface IDomSchemaSerializer
    {
        static IDomSchemaSerializer Default { get; } = new CSharpSchemaSerializer();

        string PrettyPrint(ISchema type);
        SchemaSerialization Serialize(DomSchema schema);
    }
}
