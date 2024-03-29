﻿namespace TAO3.Converters.Text;

public class TextConverter : IConverter<string>
{
    public string Format => "text";
    public IReadOnlyList<string> Aliases => new[] { "string" };
    public string MimeType => "text/plain";
    public IReadOnlyList<string> FileExtensions => new[] { ".txt" };
    public Dictionary<string, object> Properties { get; }

    public TextConverter()
    {
        Properties = new Dictionary<string, object>();
    }

    public string Deserialize(string text)
    {
        return text;
    }

    public string Serialize(object? value)
    {
        return value?.ToString() ?? string.Empty;
    }
}
