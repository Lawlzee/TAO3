namespace TAO3.TypeProvider;

public interface IDomSchemaSerializer
{
    static IDomSchemaSerializer Default { get; } = new CSharpSchemaSerializer();

    string PrettyPrint(ISchema type);
    SchemaSerialization Serialize(DomSchema schema);
}
