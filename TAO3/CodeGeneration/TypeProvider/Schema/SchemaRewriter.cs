using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public class SchemaRewriter
    {
        public virtual ISchema Visit(ISchema node)
        {
            return node.Accept(this);
        }

        public virtual TypeReferenceSchema Visit(TypeReferenceSchema node)
        {
            return new TypeReferenceSchema(
                node.Type.Accept(this),
                node.IsNullable);
        }

        public virtual ClassSchema Visit(ClassSchema node)
        {
            return new ClassSchema(
                node.FullName,
                node.Identifier,
                node.Properties
                    .Select(prop => prop.Accept(this))
                    .ToList());
        }

        public virtual ClassPropertySchema Visit(ClassPropertySchema node)
        {
            return new ClassPropertySchema(
                node.Identifier,
                node.FullName,
                node.Type.Accept(this));
        }

        public virtual CollectionTypeSchema Visit(CollectionTypeSchema node)
        {
            return new CollectionTypeSchema(
                node.InnerType.Accept(this));
        }

        public virtual LiteralTypeSchema Visit(LiteralTypeSchema node)
        {
            return node;
        }

        public virtual NullTypeSchema Visit(NullTypeSchema node)
        {
            return node;
        }

        public virtual DynamicTypeSchema Visit(DynamicTypeSchema node)
        {
            return node;
        }
    }
}
