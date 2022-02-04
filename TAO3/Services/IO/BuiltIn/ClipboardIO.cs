using System.Reactive;
using TAO3.Clipboard;

namespace TAO3.IO;

internal class ClipboardIO : ITextSource, IDestination<Unit>
{
    private readonly IClipboardService _clipboard;
    public string Name => "clipboard";

    public IReadOnlyList<string> Aliases => new[] { "cb" };

    public ClipboardIO(IClipboardService clipboard)
    {
        _clipboard = clipboard;
    }

    public async Task<string> GetTextAsync()
    {
        return (await _clipboard.GetTextAsync()) ?? string.Empty;
    }

    Task IDestination<Unit>.SetTextAsync(string text, Unit options) => SetTextAsync(text);
    public Task SetTextAsync(string text)
    {
        return _clipboard.SetTextAsync(text);
    }
}
