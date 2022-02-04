namespace TAO3.TypeProvider;

public class NullTypeSchema : ITypeSchema
{
    public bool IsValueType => false;

    ITypeSchema ITypeSchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
    public NullTypeSchema Accept(SchemaRewriter rewriter)
    {
        return rewriter.Visit(this);
    }

    public void Accept(SchemaVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override string? ToString()
    {
        return "null";
    }

    public bool AreEquivalent(ISchema obj)
    {
        return obj is NullTypeSchema schema;
    }
}
