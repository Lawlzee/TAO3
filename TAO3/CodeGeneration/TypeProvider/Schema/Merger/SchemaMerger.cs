namespace TAO3.TypeProvider;

public class SchemaMerger : ITypeReferenceSchemaMerger
{
    private readonly ILiteralTypeMerger _literalMerger;

    public SchemaMerger(ILiteralTypeMerger literalMerger)
    {
        _literalMerger = literalMerger;
    }

    public TypeReferenceSchema Merge(TypeReferenceSchema a, TypeReferenceSchema b)
    {
        bool isNullable = a.IsNullable || b.IsNullable;

        if (a.Type is NullTypeSchema || b.Type is DynamicTypeSchema)
        {
            return b.WithNullability(isNullable);
        }

        if (b.Type is NullTypeSchema || a.Type is DynamicTypeSchema)
        {
            return a.WithNullability(isNullable);
        }

        if (a.Type is LiteralTypeSchema typeA && b.Type is LiteralTypeSchema typeB)
        {
            Type mergedType = _literalMerger.Merge(typeA.Type, typeB.Type);

            return new TypeReferenceSchema(
                new LiteralTypeSchema(mergedType),
                isNullable);
        }

        if (a.Type is CollectionTypeSchema collectionA && b.Type is CollectionTypeSchema collectionB)
        {
            TypeReferenceSchema innerType = Merge(collectionA.InnerType, collectionB.InnerType);
            return new TypeReferenceSchema(
                new CollectionTypeSchema(
                    innerType),
                isNullable);
        }

        if (a.Type is CollectionTypeSchema collection1
            && (b.Type is LiteralTypeSchema || b.Type is ClassSchema))
        {
            return new TypeReferenceSchema(
                MergeWithCollection(b, collection1),
                a.IsNullable);
        }

        if (b.Type is CollectionTypeSchema collection2
            && (a.Type is LiteralTypeSchema || a.Type is ClassSchema))
        {
            return new TypeReferenceSchema(
                MergeWithCollection(a, collection2),
                b.IsNullable);
        }

        if (a.Type is ClassSchema classA && b.Type is ClassSchema classB)
        {
            return new TypeReferenceSchema(
                MergeClasses(classA, classB),
                isNullable);
        }

        //When merging class and literal
        return new TypeReferenceSchema(new DynamicTypeSchema(), isNullable);
    }

    private CollectionTypeSchema MergeWithCollection(
        TypeReferenceSchema elementSchema,
        CollectionTypeSchema collectionSchema)
    {
        return new CollectionTypeSchema(
            Merge(collectionSchema.InnerType, elementSchema));
        
    }

    private ClassSchema MergeClasses(ClassSchema classA, ClassSchema classB)
    {
        return new ClassSchema(
            classA.FullName,
            classA.Identifier,
            MergeProperties().ToList());

        IEnumerable<ClassPropertySchema> MergeProperties()
        {
            Dictionary<string, ClassPropertySchema> propertiesNotUsed = classB.Properties
                .ToDictionary(x => x.FullName);

            foreach (ClassPropertySchema propA in classA.Properties)
            {
                ClassPropertySchema? propB = propertiesNotUsed.GetValueOrDefault(propA.FullName);
                if (propB != null)
                {
                    TypeReferenceSchema newType = Merge(propA.Type, propB.Type);
                    yield return propA.WithType(newType);
                    propertiesNotUsed.Remove(propA.FullName);
                }
                else
                {
                    yield return propA;
                }
            }

            foreach (ClassPropertySchema propB in propertiesNotUsed.Values)
            {
                yield return propB;
            }
        }
    }
}
