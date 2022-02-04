namespace TAO3.TypeProvider;

public class PropertyAnnotatorContext
{
    public StringBuilder StringBuilder { get; }
    public HashSet<string> Namespaces { get; }
    public string Format { get; }
    public int Index { get; }

    public PropertyAnnotatorContext(StringBuilder stringBuilder, HashSet<string> namespaces, string format, int index)
    {
        StringBuilder = stringBuilder;
        Namespaces = namespaces;
        Format = format;
        Index = index;
    }

    public void Using(string @namespace)
    {
        Namespaces.Add(@namespace);
    }
}
