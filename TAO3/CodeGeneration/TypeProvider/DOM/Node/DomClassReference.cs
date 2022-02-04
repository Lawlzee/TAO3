using TAO3.Internal.Types;

namespace TAO3.TypeProvider;

public class DomClassReference : IDomType
{
    public string Type { get; }

    public DomClassReference(string type)
    {
        Type = type;
    }

    public DomClassReference(Type type)
    {
        Type = type.PrettyPrintFullName();
    }

    public void Accept(DomVisitor visitor)
    {
        visitor.Visit(this);
    }
}
