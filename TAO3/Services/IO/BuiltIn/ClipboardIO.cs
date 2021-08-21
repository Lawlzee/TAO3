using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.Clipboard;

namespace TAO3.IO
{
    internal class ClipboardIO : ISource<Unit>, IDestination<Unit>
    {
        private readonly IClipboardService _clipboard;
        public string Name => "clipboard";

        public IReadOnlyList<string> Aliases => new[] { "Clipboard", "cb" };

        public ClipboardIO(IClipboardService clipboard)
        {
            _clipboard = clipboard;
        }

        Task<string> ISource<Unit>.GetTextAsync(Unit options) => GetTextAsync();
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
}
