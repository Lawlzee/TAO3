using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public class ClassSchemaReplacor
    {
        private readonly Dictionary<ClassSchema, ClassSchema> _replacementRules;
        public List<ClassSchema> Classes { get; }

        public ClassSchemaReplacor(IEnumerable<ClassSchema> classes)
        {
            _replacementRules = new Dictionary<ClassSchema, ClassSchema>();
            Classes = classes.ToList();
        }

        private ClassSchema Replace(ClassSchema classSchema)
        {
            while (_replacementRules.ContainsKey(classSchema))
            {
                classSchema = _replacementRules[classSchema];
            }

            return classSchema;
        }

        public void SubstituteWith(int sourceIndex, int replacementIndex)
        {
            _replacementRules[Classes[sourceIndex]] = Classes[replacementIndex];
            Classes.RemoveAt(sourceIndex);
        }

        public void ReplaceWith(ClassSchema source, ClassSchema replacement)
        {
            ClassSchema sourceLeaf = Replace(source);
            int index = Classes.IndexOf(sourceLeaf);
            Classes[index] = replacement;

            _replacementRules[sourceLeaf] = replacement;
        }

        public ISchema Apply(ISchema schema)
        {
            return schema.Accept(new SchemaClassReplacer(this));
        }

        private class SchemaClassReplacer : SchemaRewriter
        {
            private readonly ClassSchemaReplacor _replacor;
            private readonly HashSet<ClassSchema> _classesCreated;

            public SchemaClassReplacer(ClassSchemaReplacor replacor)
            {
                _replacor = replacor;
                _classesCreated = new HashSet<ClassSchema>();
            }

            public override ClassSchema Visit(ClassSchema node)
            {
                ClassSchema replacement = _replacor.Replace(node);

                if (_classesCreated.Contains(replacement))
                {
                    return replacement;
                }

                List<ClassPropertySchema> properties = new List<ClassPropertySchema>();

                for (int i = 0; i < replacement.Properties.Count; i++)
                {
                    ClassPropertySchema prop = replacement.Properties[i].Accept(this);

                    replacement = _replacor.Replace(replacement);

                    if (_classesCreated.Contains(replacement))
                    {
                        return replacement;
                    }

                    properties.Add(prop);
                }

                ClassSchema result = new ClassSchema(
                    replacement.FullName,
                    replacement.Identifier,
                    properties);

                _replacor.ReplaceWith(node, result);
                _classesCreated.Add(result);
                return result;
            }
        }
    }
}
