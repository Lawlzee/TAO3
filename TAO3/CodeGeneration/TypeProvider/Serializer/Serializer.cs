using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Types;

namespace TAO3.TypeProvider
{
    public interface ICSharpSchemaSerializer : IDomSchemaSerializer
    {
        void AddClassAnnotator(IClassAnnotator annotator);
        void AddPropertyAnnotator(IPropertyAnnotator annotator);
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

        public void AddClassAnnotator(IClassAnnotator annotator)
        {
            _classAnnonators.Add(annotator);
        }

        public void AddPropertyAnnotator(IPropertyAnnotator annotator)
        {
            _propertyAnnonators.Add(annotator);
        }

        public SchemaSerialization Serialize(DomSchema schema)
        {
            return Serializer.Serialize(schema, _classAnnonators, _propertyAnnonators);
        }

        private class Serializer : SchemaVisitor
        {
            private readonly string _format;
            private StringBuilder _sb;
            private readonly HashSet<string> _namespaces;

            private readonly List<IClassAnnotator> _classAnnonators;
            private readonly List<IPropertyAnnotator> _propertyAnnonators;
            private readonly AnnotatorContext _context;

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
                _context = new AnnotatorContext(_sb, _namespaces, _format);
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

                //todo: cleanup
                StringBuilder codeStringBuilder = serializer._sb;
                serializer._sb = new StringBuilder();
                domSchema.Schema.Accept(serializer);
                string rootType = serializer._sb.ToString();

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

                sb.Append(codeStringBuilder);
                string code = sb.ToString();

                return new SchemaSerialization(
                    code,
                    rootType);
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

                foreach (ClassPropertySchema prop in clazz.Properties)
                {
                    Visit(prop);
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
                foreach (IPropertyAnnotator annotator in _propertyAnnonators)
                {
                    annotator.Annotate(node, _context);
                }

                Append("    public ");
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
}
