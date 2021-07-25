namespace TAO3.TypeProvider
{
    public class DomClassProperty : IDomNode
    {
        public string Name { get; }
        public IDomType Type { get; }

        public DomClassProperty(string name, IDomType type)
        {
            Name = name;
            Type = type;
        }

        public void Accept(DomVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
