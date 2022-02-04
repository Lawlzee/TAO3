namespace TAO3.TypeProvider;

public interface IDomCompiler
{
    string Format { get; }
    IDomSchematizer Schematizer { get; }
    IDomSchemaSerializer Serializer { get; }

    SchemaSerialization Compile(IDomType input);
}

public class DomCompiler : IDomCompiler
{
    public string Format { get; }
    public IDomSchematizer Schematizer { get; }
    public IDomSchemaSerializer Serializer { get; }

    public DomCompiler(string format, IDomSchematizer schematizer, IDomSchemaSerializer serializer)
    {
        Format = format;
        Schematizer = schematizer;
        Serializer = serializer;
    }

    public SchemaSerialization Compile(IDomType input)
    {
        DomSchema schema = Schematizer.Schematize(input, Format);
        SchemaSerialization serialization = Serializer.Serialize(schema);
        return serialization;
    }
}
