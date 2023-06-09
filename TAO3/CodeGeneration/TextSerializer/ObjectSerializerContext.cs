namespace TAO3.TextSerializer;

public class ObjectSerializerContext<TSettings> 
{
    private readonly int _indentationLevel;
    private readonly string _indentationString;
    private readonly StringBuilder _stringBuilder;
    private readonly ObjectSerializer<TSettings> _serializer;

    public TSettings Settings;

    private string? _identation;
    public string Indentation => _identation ??= string.Concat(Enumerable.Repeat(_indentationString, _indentationLevel));

    public ObjectSerializerContext(int indentationLevel, string indentationString, TSettings settings, StringBuilder stringBuilder, ObjectSerializer<TSettings> serializer)
    {
        _indentationLevel = indentationLevel;
        _indentationString = indentationString;
        Settings = settings;
        _stringBuilder = stringBuilder;
        _serializer = serializer;
    }

    public ObjectSerializerContext<TSettings> Indent()
    {
        return new ObjectSerializerContext<TSettings>(
            _indentationLevel + 1,
            _indentationString,
            Settings,
            _stringBuilder,
            _serializer);
    }

    public StringBuilder Append(string? text)
    {
        return _stringBuilder.Append(text);
    }

    public StringBuilder Append(char c)
    {
        return _stringBuilder.Append(c);
    }

    public StringBuilder Append(int c)
    {
        return _stringBuilder.Append(c);
    }

    public StringBuilder AppendLine()
    {
        return _stringBuilder.AppendLine();
    }

    public StringBuilder AppendLine(string? text)
    {
        return _stringBuilder.AppendLine(text);
    }

    public StringBuilder AppendIndentation()
    {
        return _stringBuilder.Append(Indentation);
    }

    public void Serialize(object? obj)
    {
        _serializer.Serialize(obj, this);
    }
}
