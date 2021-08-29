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
            ITypeSchema type = node.Type.Accept(this);

            if (node.Type == type)
            {
                return node;
            }

            return new TypeReferenceSchema(
                type,
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
            TypeReferenceSchema type = node.Type.Accept(this);

            if (node.Type == type)
            {
                return node;
            }

            return new ClassPropertySchema(
                node.Identifier,
                node.FullName,
                type);
        }

        public virtual CollectionTypeSchema Visit(CollectionTypeSchema node)
        {
            TypeReferenceSchema innerType = node.InnerType.Accept(this);

            if (innerType == node.InnerType)
            {
                return node;
            }

            return new CollectionTypeSchema(innerType);
        }

        public virtual ClassReferenceSchema Visit(ClassReferenceSchema node)
        {
            return node;
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
