using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public class SchemaSerialization
    {
        public string Code { get; }
        public ISchema Root { get; }
        public ISchema? RootElementType { get; }

        public SchemaSerialization(string code, ISchema root)
        {
            Code = code;
            Root = root;
            if (Root is CollectionTypeSchema collection)
            {
                RootElementType = collection.InnerType;
            }
        }
    }
}
