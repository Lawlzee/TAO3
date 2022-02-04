namespace TAO3.IO;

public interface ISource
{
    string Name { get; }
    IReadOnlyList<string> Aliases { get; }
}
