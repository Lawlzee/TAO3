using System.Collections.Generic;

namespace TAO3.TypeProvider
{
    public class ClassSchema : ITypeSchema
    {
        public string Identifier { get; }
        public List<ClassPropertySchema> Properties { get; }
        public bool IsValueType { get; }

        public ClassSchema(string identifier, List<ClassPropertySchema> properties)
        {
            Identifier = identifier;
            Properties = properties;
        }

        ITypeSchema ITypeSchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
        public ClassSchema Accept(SchemaRewriter rewriter)
        {
            return rewriter.Visit(this);
        }

        public void Accept(SchemaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
