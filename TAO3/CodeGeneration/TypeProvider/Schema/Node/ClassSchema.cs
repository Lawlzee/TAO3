using System.Collections.Generic;

namespace TAO3.TypeProvider
{
    public class ClassSchema : ITypeSchema
    {
        public string FullName { get; }
        public string Identifier { get; }
        public List<ClassPropertySchema> Properties { get; }
        public bool IsValueType => false;

        public ClassSchema(string fullName, string identifier, List<ClassPropertySchema> properties)
        {
            FullName = fullName;
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
