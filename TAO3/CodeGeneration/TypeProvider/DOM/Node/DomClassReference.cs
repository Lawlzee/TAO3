using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public class DomClassReference : IDomType
    {
        public string Type { get; }

        public DomClassReference(string type)
        {
            Type = type;
        }

        public DomClassReference(Type type)
        {
            Type = type.FullName!;
        }

        public void Accept(DomVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
