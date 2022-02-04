namespace TAO3.TypeProvider;

public interface ISchema
{
    ISchema Accept(SchemaRewriter rewriter);
    void Accept(SchemaVisitor visitor);
    bool AreEquivalent(ISchema other);
}
