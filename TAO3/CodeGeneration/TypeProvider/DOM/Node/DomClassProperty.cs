namespace TAO3.TypeProvider
{
    public class DomClassProperty : IDomNode
    {
        public string Identifier { get; }
        public string Name { get; }
        public IDomType Type { get; }

        public DomClassProperty(string identifier, string name, IDomType type)
        {
            Identifier = identifier;
            Name = name;
            Type = type;
        }

        public DomClassProperty(string name, IDomType type)
        {
            Identifier = name;
            Name = name;
            Type = type;
        }

        public void Accept(DomVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
