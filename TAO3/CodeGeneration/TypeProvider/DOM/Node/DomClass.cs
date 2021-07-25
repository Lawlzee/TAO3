using System.Collections.Generic;

namespace TAO3.TypeProvider
{
    public class DomClass : IDomType
    {
        public string Name { get; }
        public List<DomClassProperty> Properties { get; }

        public DomClass(string name, List<DomClassProperty> properties)
        {
            Name = name;
            Properties = properties;
        }

        public void Accept(DomVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
