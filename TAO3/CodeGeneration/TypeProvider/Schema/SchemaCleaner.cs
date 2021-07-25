using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TAO3.CodeGeneration;
using TAO3.Internal.CodeGeneration;

namespace TAO3.TypeProvider
{
    public interface ISchemaCleaner
    {
        static ISchemaCleaner Default { get; } = new SchemaCleaner(IClassSchemaMerger.Default);
        DomSchema Clean(ISchema schema, string format);
    }

    //todo: refactor
    public class SchemaCleaner : ISchemaCleaner
    {
        private readonly IClassSchemaMerger _classMerger;

        public SchemaCleaner(IClassSchemaMerger classMerger)
        {
            _classMerger = classMerger;
        }

        public DomSchema Clean(ISchema schema, string format)
        {
            List<ClassSchema> classes = ClassSchemaFinder.FindClasses(schema);
            Dictionary<ClassSchema, ClassSchema> replacementRules = InferReplacementRules(classes);
            return DeduplicatorRewritter.RemoveDuplicates(schema, format, replacementRules);
        }

        private Dictionary<ClassSchema, ClassSchema> InferReplacementRules(List<ClassSchema> classes)
        {
            List<ClassSchema> classesLeft = classes.ToList();
            Dictionary<ClassSchema, ClassSchema> replacementRules = new Dictionary<ClassSchema, ClassSchema>();

            bool found;
            do
            {
                found = false;
                for (int i = 0; i < classesLeft.Count; i++)
                {
                    for (int j = i + 1; j < classesLeft.Count; j++)
                    {
                        ClassSchema classI = classesLeft[i];
                        ClassSchema classJ = classesLeft[i];

                        if (HasSamePropertiesNames(classI, classJ))
                        {
                            ClassSchema? merged = HasCompatiblesProperties(classI, classJ);
                            if (merged != null)
                            {
                                found = true;
                                classesLeft.RemoveAt(j);
                                classesLeft[i] = merged;

                                replacementRules[classI] = merged;
                                replacementRules[classJ] = merged;
                            }
                        }
                    }
                }
            }
            while (found);

            RemoveDuplicatesClassNames(classesLeft, replacementRules);

            return replacementRules;
        }

        private static void RemoveDuplicatesClassNames(
            List<ClassSchema> classesLeft, 
            Dictionary<ClassSchema, ClassSchema> replacementRules)
        {
            HashSet<string> identifiersUsed = new HashSet<string>();

            foreach (ClassSchema clazz in classesLeft)
            {
                if (identifiersUsed.Contains(clazz.Identifier))
                {
                    string newIdentifier = IdentifierUtils.GetUniqueIdentifier(clazz.Identifier, identifiersUsed);
                    replacementRules[clazz] = new ClassSchema(
                        newIdentifier,
                        clazz.Properties);

                    identifiersUsed.Add(newIdentifier);
                }
                else
                {
                    identifiersUsed.Add(clazz.Identifier);
                }

            }
        }

        //Todo: we can propably do something better than this...
        private bool HasSamePropertiesNames(ClassSchema classA, ClassSchema classB)
        {
            HashSet<string> propsA = classA.Properties.Select(x => x.FullName).ToHashSet();
            HashSet<string> propsB = classB.Properties.Select(x => x.FullName).ToHashSet();

            return propsA.SetEquals(propsB);
        }

        //Todo: we can propably do something better than this...
        private ClassSchema? HasCompatiblesProperties(ClassSchema classA, ClassSchema classB)
        {
            ClassSchema merged = _classMerger.MergeClasses(classA, classB);

            foreach (ClassPropertySchema mergedProp in merged.Properties)
            {
                if (!(mergedProp.Type.Type is DynamicTypeSchema))
                {
                    continue;
                }

                ClassPropertySchema? propA = classA.Properties
                    .FirstOrDefault(x => x.FullName == mergedProp.FullName);

                if (propA != null && propA.Type.Type is DynamicTypeSchema)
                {
                    continue;
                }

                ClassPropertySchema? propB = classB.Properties
                    .FirstOrDefault(x => x.FullName == mergedProp.FullName);

                if (propB != null && propB.Type.Type is DynamicTypeSchema)
                {
                    continue;
                }

                return null;
            }

            return merged;
        }

        private class DeduplicatorRewritter : SchemaRewriter
        {
            private readonly Dictionary<ClassSchema, ClassSchema> _replacementRules;
            private readonly List<ClassSchema> _classes;
            private HashSet<string> _currentUsedPropertyIdentifier;

            private DeduplicatorRewritter(Dictionary<ClassSchema, ClassSchema> replacementRules)
            {
                _replacementRules = replacementRules;
                _classes = new List<ClassSchema>();
                _currentUsedPropertyIdentifier = new HashSet<string>();
            }

            public static DomSchema RemoveDuplicates(
                ISchema schema,
                string format,
                Dictionary<ClassSchema, ClassSchema> replacementRules)
            {
                DeduplicatorRewritter rewritter = new DeduplicatorRewritter(replacementRules);
                ISchema resultSchema = schema.Accept(rewritter);
                return new DomSchema(
                    format,
                    resultSchema,
                    rewritter._classes);
            }

            public override ClassSchema Visit(ClassSchema node)
            {
                ClassSchema? replacement;
                while (true)
                {
                    replacement = _replacementRules.GetValueOrDefault(node);
                    if (replacement == null)
                    {
                        break;
                    }
                    node = replacement;
                }

                HashSet<string> oldPropertyIdenfiers = _currentUsedPropertyIdentifier;
                _currentUsedPropertyIdentifier = new HashSet<string>();

                ClassSchema result = base.Visit(node);
                _classes.Add(result);

                _currentUsedPropertyIdentifier = oldPropertyIdenfiers;

                return result;
            }

            public override ClassPropertySchema Visit(ClassPropertySchema node)
            {
                if (_currentUsedPropertyIdentifier.Contains(node.Identifier))
                {
                    string newIdentifier = IdentifierUtils.GetUniqueIdentifier(node.Identifier, _currentUsedPropertyIdentifier);
                    _currentUsedPropertyIdentifier.Add(newIdentifier);

                    return new ClassPropertySchema(
                        newIdentifier,
                        node.FullName,
                        node.Type.Accept(this));
                }

                _currentUsedPropertyIdentifier.Add(node.Identifier);
                return base.Visit(node);
            }
        }
    }
}
