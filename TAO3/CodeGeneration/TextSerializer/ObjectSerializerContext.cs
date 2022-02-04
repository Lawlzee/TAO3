namespace TAO3.TextSerializer;

public record ObjectSerializerContext<TSettings>(
    int IndentationLevel,
    string IndentationString,
    TSettings Settings,
    StringBuilder StringBuilder,
    ObjectSerializer<TSettings> Serializer)
{
    private string? _identation;
    public string Indentation => _identation ??= string.Concat(Enumerable.Repeat(IndentationString, IndentationLevel));

    public ObjectSerializerContext<TSettings> Indent()
    {
        return this with
        {
            IndentationLevel = IndentationLevel + 1,
        };
    }

    public StringBuilder Append(string? text)
    {
        return StringBuilder.Append(text);
    }

    public StringBuilder Append(char c)
    {
        return StringBuilder.Append(c);
    }

    public StringBuilder Append(int c)
    {
        return StringBuilder.Append(c);
    }

    public StringBuilder AppendLine()
    {
        return StringBuilder.AppendLine();
    }

    public StringBuilder AppendLine(string? text)
    {
        return StringBuilder.AppendLine(text);
    }

    public StringBuilder AppendIndentation()
    {
        return StringBuilder.Append(Indentation);
    }

    public void Serialize(object? obj)
    {
        Serializer.Serialize(obj, this);
    }
}
