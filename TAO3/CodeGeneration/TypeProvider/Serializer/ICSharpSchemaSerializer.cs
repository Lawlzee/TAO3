using TAO3.Internal.Types;

namespace TAO3.TypeProvider;

public interface ICSharpSchemaSerializer : IDomSchemaSerializer
{
    void AddAnnotator(IClassAnnotator annotator);
    void AddAnnotator(IPropertyAnnotator annotator);
}

public class CSharpSchemaSerializer : ICSharpSchemaSerializer
{
    private readonly List<IClassAnnotator> _classAnnonators;
    private readonly List<IPropertyAnnotator> _propertyAnnonators;

    public CSharpSchemaSerializer()
    {
        _classAnnonators = new List<IClassAnnotator>();
        _propertyAnnonators = new List<IPropertyAnnotator>();
    }

    public void AddAnnotator(IClassAnnotator annotator)
    {
        _classAnnonators.Add(annotator);
    }

    public void AddAnnotator(IPropertyAnnotator annotator)
    {
        _propertyAnnonators.Add(annotator);
    }

    public SchemaSerialization Serialize(DomSchema schema)
    {
        return Serializer.Serialize(schema, _classAnnonators, _propertyAnnonators);
    }

    public string PrettyPrint(ISchema type)
    {
        return Serializer.PrettyPrint(type);
    }

    private class Serializer : SchemaVisitor
    {
        private readonly string _format;
        private StringBuilder _sb;
        private readonly HashSet<string> _namespaces;
        private int _propertyIndex;

        private readonly List<IClassAnnotator> _classAnnonators;
        private readonly List<IPropertyAnnotator> _propertyAnnonators;
        private readonly ClassAnnotatorContext _context;

        private Serializer(
            string format,
            List<IClassAnnotator> classAnnonators,
            List<IPropertyAnnotator> propertyAnnonators)
        {
            _format = format;
            _sb = new StringBuilder();
            _namespaces = new HashSet<string>();
            _classAnnonators = classAnnonators;
            _propertyAnnonators = propertyAnnonators;
            _context = new ClassAnnotatorContext(_sb, _namespaces, _format);
        }

        public static SchemaSerialization Serialize(
            DomSchema domSchema,
            List<IClassAnnotator> classAnnonators,
            List<IPropertyAnnotator> propertyAnnonators)
        {
            Serializer serializer = new Serializer(
                domSchema.Format,
                classAnnonators,
                propertyAnnonators);

            bool isFirst = true;
            foreach (ClassSchema clazz in domSchema.Classes)
            {
                if (!isFirst)
                {
                    serializer.AppendLine();
                    serializer.AppendLine();
                }
                serializer.SerializeClass(clazz);
                isFirst = false;
            }

            StringBuilder sb = new StringBuilder();
            foreach (string @namespace in serializer._namespaces.OrderBy(x => x))
            {
                sb.Append("using ");
                sb.Append(@namespace);
                sb.AppendLine(";");
            }

            if (serializer._namespaces.Count > 0)
            {
                sb.AppendLine();
            }

            sb.Append(serializer._sb);
            string code = sb.ToString();

            return new SchemaSerialization(
                code,
                domSchema.Schema);
        }

        public static string PrettyPrint(ISchema type)
        {
            Serializer serializer = new Serializer(
                "",
                new List<IClassAnnotator>(),
                new List<IPropertyAnnotator>());

            type.Accept(serializer);
            return serializer._sb.ToString();
        }

        private void SerializeClass(ClassSchema clazz)
        {
            foreach (IClassAnnotator annotator in _classAnnonators)
            {
                annotator.Annotate(clazz, _context);
            }

            Append("public class ");
            Append(clazz.Identifier);
            AppendLine();
            AppendLine("{");

            _propertyIndex = 0;
            foreach (ClassPropertySchema prop in clazz.Properties)
            {
                if (_propertyIndex > 0)
                {
                    AppendLine();
                }

                Visit(prop);
                _propertyIndex++;
            }

            Append("}");
        }

        public override void Visit(TypeReferenceSchema node)
        {
            base.Visit(node);
            if (node.IsNullable)
            {
                Append("?");
            }
        }

        public override void Visit(ClassSchema node)
        {
            Append(node.Identifier);
        }

        public override void Visit(ClassPropertySchema node)
        {
            Append("    ");

            foreach (IPropertyAnnotator annotator in _propertyAnnonators)
            {
                annotator.Annotate(node, new PropertyAnnotatorContext(
                    _sb,
                    _namespaces,
                    _format,
                    _propertyIndex));
            }

            Append("public ");
            base.Visit(node);

            Append(" ");
            Append(node.Identifier);
            Append(" { get; set; }");
            if (!node.Type.Type.IsValueType)
            {
                Append(" = null!;");
            }
            AppendLine();
        }

        public override void Visit(CollectionTypeSchema node)
        {
            Using("System.Collections.Generic");

            Append("List<");
            base.Visit(node);
            Append(">");
        }

        public override void Visit(LiteralTypeSchema node)
        {
            Using(node.Type.Namespace!);
            Append(node.Type.PrettyPrint());
        }

        public override void Visit(ClassReferenceSchema node)
        {
            Append(node.Type);
        }

        public override void Visit(NullTypeSchema node)
        {
            Append("object");
        }

        public override void Visit(DynamicTypeSchema node)
        {
            Append("dynamic");
        }

        private void Append(string str)
        {
            _sb.Append(str);
        }

        private void AppendLine(string str)
        {
            _sb.AppendLine(str);
        }

        private void AppendLine()
        {
            _sb.AppendLine();
        }

        private void Using(string @namespace)
        {
            _namespaces.Add(@namespace);
        }
    }
}
