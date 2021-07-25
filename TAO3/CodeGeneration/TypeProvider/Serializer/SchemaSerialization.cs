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
        public string RootType { get; }

        public SchemaSerialization(string code, string rootType)
        {
            Code = code;
            RootType = rootType;
        }
    }
}
