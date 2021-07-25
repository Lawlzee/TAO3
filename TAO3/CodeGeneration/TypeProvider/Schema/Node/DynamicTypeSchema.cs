namespace TAO3.TypeProvider
{
    public class DynamicTypeSchema : ITypeSchema
    {
        public bool IsValueType => false;

        ITypeSchema ITypeSchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
        public DynamicTypeSchema Accept(SchemaRewriter rewriter)
        {
            return rewriter.Visit(this);
        }

        public void Accept(SchemaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
