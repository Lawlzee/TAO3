namespace TAO3.TypeProvider
{
    public interface IDomNode
    {
        void Accept(DomVisitor visitor);
    }
}
