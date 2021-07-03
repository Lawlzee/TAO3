using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Clipboard;

namespace TAO3.InputSources
{
    internal class ClipboardInputSource : IInputSource
    {
        private readonly IClipboardService _clipboard;
        public string Name => "clipboard";

        public ClipboardInputSource(IClipboardService clipboard)
        {
            _clipboard = clipboard;
        }

        public async Task<string> GetTextAsync()
        {
            return (await _clipboard.GetTextAsync()) ?? string.Empty;
        }
    }
}
