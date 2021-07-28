using System;
using System.Collections.Generic;
using System.Linq;

namespace TAO3.TypeProvider
{
    public class ClassSchema : ITypeSchema
    {
        public string FullName { get; }
        public string Identifier { get; }
        public List<ClassPropertySchema> Properties { get; }
        public bool IsValueType => false;

        public ClassSchema(string fullName, string identifier, List<ClassPropertySchema> properties)
        {
            FullName = fullName;
            Identifier = identifier;
            Properties = properties;
        }

        ITypeSchema ITypeSchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
        public ClassSchema Accept(SchemaRewriter rewriter)
        {
            return rewriter.Visit(this);
        }

        public void Accept(SchemaVisitor visitor)
        {
            visitor.Visit(this);
        }



        public override string? ToString()
        {
            return $"Class {Identifier} ({FullName})";
        }

        public bool AreEquivalent(ISchema obj)
        {
            return obj is ClassSchema schema &&
                FullName == schema.FullName &&
                Identifier == schema.Identifier &&
                Properties.Count ==  schema.Properties.Count &&
                Properties.OrderBy(x => x.FullName)
                    .Zip(schema.Properties.OrderBy(x => x.FullName))
                    .All(x => x.First.AreEquivalent(x.Second)) &&
                IsValueType == schema.IsValueType;
        }
    }
}
