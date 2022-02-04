namespace TAO3.Formatting;

public interface IFormatter
{
    string Format(string text);
    string Format(string text, object? config)
    {
        return Format(text);
    }
}

public interface IFormatter<TConfig> : IFormatter
{
    string Format(string text, TConfig? config);

    string IFormatter.Format(string text, object? config)
    {
        return Format(text, (TConfig)config!);
    }

    string IFormatter.Format(string text)
    {
        return Format(text, default(TConfig));
    }
}
