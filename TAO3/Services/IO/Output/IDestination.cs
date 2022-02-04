namespace TAO3.IO;

public interface IDestination
{
    string Name { get; }
    IReadOnlyList<string> Aliases { get; }
}

public interface IDestination<TOptions> : IDestination
{
    Task SetTextAsync(string text, TOptions options);
}
