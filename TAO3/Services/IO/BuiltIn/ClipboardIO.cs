using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Clipboard;

namespace TAO3.IO
{
    internal class ClipboardIO : ISource, IDestination
    {
        private readonly IClipboardService _clipboard;
        public string Name => "clipboard";

        public IReadOnlyList<string> Aliases => new[] { "Clipboard", "cb" };

        public ClipboardIO(IClipboardService clipboard)
        {
            _clipboard = clipboard;
        }

        public async Task<string> GetTextAsync()
        {
            return (await _clipboard.GetTextAsync()) ?? string.Empty;
        }

        public Task SetTextAsync(string text)
        {
            return _clipboard.SetTextAsync(text);
        }
    }
}
