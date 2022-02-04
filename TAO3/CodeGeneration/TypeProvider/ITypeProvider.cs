namespace TAO3.TypeProvider;

public interface ITypeProvider<TInput> : IDomCompiler
{
    IDomParser<TInput> DomParser { get; }
    SchemaSerialization ProvideTypes(TInput input);
}

public class TypeProvider<TInput> : ITypeProvider<TInput>
{
    public string Format { get; }
    public IDomParser<TInput> DomParser { get; }
    public IDomSchematizer Schematizer { get; }
    public IDomSchemaSerializer Serializer { get; }

    public TypeProvider(
        string format, 
        IDomParser<TInput> domParser, 
        IDomSchematizer schematizer, 
        IDomSchemaSerializer serializer)
    {
        Format = format;
        DomParser = domParser;
        Schematizer = schematizer;
        Serializer = serializer;
    }

    public SchemaSerialization ProvideTypes(TInput input)
    {
        IDomType dom = DomParser.Parse(input);
        DomSchema schema = Schematizer.Schematize(dom, Format);
        SchemaSerialization serialization = Serializer.Serialize(schema);
        return serialization;
    }

    public SchemaSerialization Compile(IDomType input)
    {
        DomSchema schema = Schematizer.Schematize(input, Format);
        SchemaSerialization serialization = Serializer.Serialize(schema);
        return serialization;
    }
}
