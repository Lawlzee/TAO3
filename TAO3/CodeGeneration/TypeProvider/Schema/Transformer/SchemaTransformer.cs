namespace TAO3.TypeProvider;

public interface ISchemaTransformer
{
    static ISchemaTransformer Default { get; } = new SchemaTransformer(new List<ISchemaTransformation>
    {
        new RemoveDuplicatedClassTransformer(),
        new RenameEmptyClassNameTransformer(),
        new RenameDuplicatedClassNamesTransformer(),
        new RenameEmptyPropertyNameTransformer(),
        new RenameDuplicatedPropertyNamesTransformer()
    });
    ISchema Transform(ISchema schema);
}

public class SchemaTransformer : ISchemaTransformer
{
    private readonly List<ISchemaTransformation> _transformations;

    public SchemaTransformer(List<ISchemaTransformation> transformations)
    {
        _transformations = transformations;
    }

    public ISchema Transform(ISchema schema)
    {
        List<ClassSchema> classes = ClassSchemaFinder.FindClasses(schema);

        ClassSchemaReplacor replacor = new ClassSchemaReplacor(classes);

        foreach (ISchemaTransformation transformation in _transformations)
        {
            transformation.Transform(replacor);
        }

        return replacor.Apply(schema);
    }
}
