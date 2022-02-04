namespace TAO3.TypeProvider;

public class ClassReferenceSchema : ITypeSchema
{
    public string Type { get; }
    public bool IsValueType => false;

    public ClassReferenceSchema(string type)
    {
        Type = type;
    }

    ITypeSchema ITypeSchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
    public ClassReferenceSchema Accept(SchemaRewriter rewriter)
    {
        return rewriter.Visit(this);
    }

    public void Accept(SchemaVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override string? ToString()
    {
        return Type;
    }

    public bool AreEquivalent(ISchema obj)
    {
        return obj is ClassReferenceSchema schema &&
               Type == schema.Type;
    }
}
