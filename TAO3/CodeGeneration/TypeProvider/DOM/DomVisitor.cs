namespace TAO3.TypeProvider;

public abstract class DomVisitor
{
    public virtual void Visit(IDomNode node)
    {
        node.Accept(this);
    }

    public virtual void Visit(DomClass node)
    {
        foreach (DomClassProperty property in node.Properties)
        {
            property.Accept(this);
        }
    }

    public virtual void Visit(DomClassProperty node)
    {

    }

    public virtual void Visit(DomClassReference node)
    {

    }

    public virtual void Visit(DomLiteral node)
    {

    }

    public virtual void Visit(DomCollection node)
    {
        foreach (IDomType value in node.Values)
        {
            value.Accept(this);
        }
    }

    public virtual void Visit(DomNullLiteral node)
    {

    }
}
