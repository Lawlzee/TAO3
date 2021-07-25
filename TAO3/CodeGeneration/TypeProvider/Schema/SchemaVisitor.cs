using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TypeProvider;

namespace TAO3.TypeProvider
{
    public class SchemaVisitor
    {
        public virtual void Visit(ISchema node)
        {
            node.Accept(this);
        }

        public virtual void Visit(TypeReferenceSchema node)
        {
            node.Type.Accept(this);
        }

        public virtual void Visit(ClassSchema node)
        {
            foreach (var prop in node.Properties)
            {
                prop.Accept(this);
            }
        }

        public virtual void Visit(ClassPropertySchema node)
        {
            node.Type.Accept(this);
        }

        public virtual void Visit(CollectionTypeSchema node)
        {
            node.InnerType.Accept(this);
        }

        public virtual void Visit(LiteralTypeSchema node)
        {

        }

        public virtual void Visit(NullTypeSchema node)
        {

        }

        public virtual void Visit(DynamicTypeSchema node)
        {

        }
    }
}
