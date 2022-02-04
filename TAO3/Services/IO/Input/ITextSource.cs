using System.Reactive;

namespace TAO3.IO;

public interface ITextSource<TOptions> : ISource
{
    Task<string> GetTextAsync(TOptions options);
}

public interface ITextSource : ITextSource<Unit>
{
    Task<string> ITextSource<Unit>.GetTextAsync(Unit options) => GetTextAsync();
    Task<string> GetTextAsync();
}
