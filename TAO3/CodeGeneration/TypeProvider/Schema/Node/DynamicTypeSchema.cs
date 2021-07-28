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

        public override string? ToString()
        {
            return "dynamic";
        }

        public bool AreEquivalent(ISchema obj)
        {
            return obj is DynamicTypeSchema;
        }
    }
}
