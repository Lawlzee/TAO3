using System;
using System.Collections.Generic;

namespace TAO3.TypeProvider
{
    public class TypeReferenceSchema : ISchema
    {
        public ITypeSchema Type { get; }
        public bool IsNullable { get; }

        public TypeReferenceSchema(ITypeSchema type, bool isNullable)
        {
            Type = type;
            IsNullable = isNullable;
        }

        public TypeReferenceSchema WithNullability(bool nullable)
        {
            return new TypeReferenceSchema(
                Type,
                nullable);
        }

        ISchema ISchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
        public TypeReferenceSchema Accept(SchemaRewriter rewriter)
        {
            return rewriter.Visit(this);
        }

        public void Accept(SchemaVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string? ToString()
        {
            if (IsNullable)
            {
                return $"{Type}?";
            }

            return Type.ToString();
        }

        public bool AreEquivalent(ISchema obj)
        {
            return obj is TypeReferenceSchema schema &&
                   Type.AreEquivalent(schema.Type) &&
                   IsNullable == schema.IsNullable;
        }
    }
}
