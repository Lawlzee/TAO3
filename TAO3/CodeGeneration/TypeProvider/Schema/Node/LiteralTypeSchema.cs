using System;

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
    }
}
