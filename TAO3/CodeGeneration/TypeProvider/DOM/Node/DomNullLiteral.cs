namespace TAO3.TypeProvider
{
    public class DomNullLiteral : IDomType
    {
        public void Accept(DomVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
