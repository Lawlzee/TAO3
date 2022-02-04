namespace TAO3.TypeProvider;

public interface IDomSchematizer
{
    static IDomSchematizer Default { get; } = new DomSchematizer(IDomReducer.Default, ISchemaTransformer.Default);
    DomSchema Schematize(IDomType node, string format);
}

public class DomSchematizer : IDomSchematizer
{
    private readonly IDomReducer _domReducer;
    private readonly ISchemaTransformer _schemaTransformer;

    public DomSchematizer(IDomReducer domReducer, ISchemaTransformer schemaTransformer)
    {
        _domReducer = domReducer;
        _schemaTransformer = schemaTransformer;
    }

    public DomSchema Schematize(IDomType node, string format)
    {
        ITypeSchema schema = _domReducer.Reduce(node);
        ISchema transformedSchema = _schemaTransformer.Transform(schema);
        return new DomSchema(
            format,
            transformedSchema,
            ClassSchemaFinder.FindClasses(transformedSchema));
    }
}
