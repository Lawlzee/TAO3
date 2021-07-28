using System;
using System.Collections.Generic;

namespace TAO3.TypeProvider
{
    public class CollectionTypeSchema : ITypeSchema
    {
        public TypeReferenceSchema InnerType { get; }
        public bool IsValueType => false;

        public CollectionTypeSchema(TypeReferenceSchema innerType)
        {
            InnerType = innerType;
        }

        ITypeSchema ITypeSchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
        public CollectionTypeSchema Accept(SchemaRewriter rewriter)
        {
            return rewriter.Visit(this);
        }

        public void Accept(SchemaVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string? ToString()
        {
            return $"Collection {InnerType}";
        }

        public bool AreEquivalent(ISchema obj)
        {
            return obj is CollectionTypeSchema schema &&
                   InnerType.AreEquivalent(schema.InnerType);
        }
    }
}
