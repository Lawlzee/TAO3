using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Interop;

namespace TAO3.InputSources
{
    internal class ClipboardInputSource : IInputSource
    {
        private readonly IClipboard _clipboard;
        public string Name => "clipboard";

        public ClipboardInputSource(IClipboard clipboard)
        {
            _clipboard = clipboard;
        }

        public async Task<string> GetText(string source, KernelInvocationContext context)
        {
            return await _clipboard.GetTextAsync();
        }
    }
}
