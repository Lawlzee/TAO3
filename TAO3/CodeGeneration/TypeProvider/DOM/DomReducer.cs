using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.CodeGeneration;
using TAO3.Internal.CodeGeneration;

namespace TAO3.TypeProvider
{
    public interface IDomReducer
    {
        static IDomReducer Default { get; } = new DomReducer(ITypeReferenceSchemaMerger.Default);
        ITypeSchema Reduce(IDomType node);
    }

    public class DomReducer : DomVisitor, IDomReducer
    {
        private readonly ITypeReferenceSchemaMerger _merger;
        private List<ClassPropertySchema>? _properties;
        private TypeReferenceSchema? _currentSchema;

        public DomReducer(ITypeReferenceSchemaMerger merger)
        {
            _merger = merger;
        }

        public ITypeSchema Reduce(IDomType node)
        {
            node.Accept(this);
            return _currentSchema?.Type!;
        }

        public override void Visit(DomClass node)
        {
            List<ClassPropertySchema>? oldProperties = _properties;
            _properties = new List<ClassPropertySchema>();
            base.Visit(node);

            //Merge properties with same names
            List<ClassPropertySchema> properties = _properties
                .GroupBy(x => x.FullName)
                .Select(grp => grp
                    .Aggregate((a, b) => a.WithType(_merger.Merge(a.Type, b.Type))))
                .ToList();

            _currentSchema = new TypeReferenceSchema(
                new ClassSchema(
                    node.Name,
                    IdentifierUtils.ToCSharpIdentifier(node.Name),
                    properties),
                false);

            _properties = oldProperties;
        }

        public override void Visit(DomClassProperty node)
        {
            node.Type.Accept(this);
            _properties!.Add(
                new ClassPropertySchema(
                    IdentifierUtils.ToCSharpIdentifier(node.Name),
                    node.Name,
                    _currentSchema!));
        }

        public override void Visit(DomLiteral node)
        {
            _currentSchema = new TypeReferenceSchema(
                new LiteralTypeSchema(node.Type),
                false);
        }

        public override void Visit(DomCollection node)
        {
            TypeReferenceSchema innerType = node
                .Values
                .Select(element =>
                {
                    element.Accept(this);
                    return _currentSchema!;
                })
                .Aggregate((a, b) => _merger.Merge(a, b));

            _currentSchema = new TypeReferenceSchema(new CollectionTypeSchema(innerType), false);
        }

        public override void Visit(DomNullLiteral node)
        {
            _currentSchema = new TypeReferenceSchema(new NullTypeSchema(), true);
        }
    }
}
