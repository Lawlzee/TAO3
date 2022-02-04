using System.Drawing;
using System.Reactive;
using System.Threading;

namespace TAO3.Clipboard;

public interface IClipboardService : IDisposable
{
    IObservable<Unit> OnClipboardChange { get; }

    string? GetText();
    void SetText(string text);

    Task<string?> GetTextAsync();
    Task<string?> GetTextAsync(CancellationToken cancellation);
    Task SetTextAsync(string text);
    Task SetTextAsync(string text, CancellationToken cancellation);

    Task<Image?> GetImageAsync();

    void Clear();
    Task<List<string>> GetFilesAsync();
}
