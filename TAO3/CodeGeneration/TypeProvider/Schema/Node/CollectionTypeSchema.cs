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
    }
}
