using System;
using System.Collections.Generic;
using TAO3.Internal.Types;

namespace TAO3.TypeProvider
{
    public class LiteralTypeSchema : ITypeSchema
    {
        public Type Type { get; }
        public bool IsValueType => !Type.IsClass;

        public LiteralTypeSchema(Type type)
        {
            Type = type;
        }

        ITypeSchema ITypeSchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
        public LiteralTypeSchema Accept(SchemaRewriter rewriter)
        {
            return rewriter.Visit(this);
        }

        public void Accept(SchemaVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string? ToString()
        {
            return Type.PrettyPrint();
        }

        public bool AreEquivalent(ISchema obj)
        {
            return obj is LiteralTypeSchema schema &&
                   Type == schema.Type;
        }
    }
}
