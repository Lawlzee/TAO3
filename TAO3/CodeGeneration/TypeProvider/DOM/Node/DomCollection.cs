namespace TAO3.TypeProvider;

public class DomCollection : IDomType
{
    public List<IDomType> Values { get; }

    public DomCollection(List<IDomType> values)
    {
        Values = values;
    }

    public void Accept(DomVisitor visitor)
    {
        visitor.Visit(this);
    }
}
