namespace TAO3.TypeProvider;

public class DomLiteral : IDomType
{
    public Type Type { get; }

    public DomLiteral(Type type)
    {
        Type = type;
    }

    public void Accept(DomVisitor visitor)
    {
        visitor.Visit(this);
    }
}
