using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Clipboard;

namespace TAO3.OutputDestinations
{
    internal class ClipboardOutputDestination : IOutputDestination
    {
        private readonly IClipboardService _clipboard;

        public string Name => "clipboard";

        public ClipboardOutputDestination(IClipboardService clipboard)
        {
            _clipboard = clipboard;
        }

        public Task SetTextAsync(string text, KernelInvocationContext context)
        {
            return _clipboard.SetTextAsync(text);
        }
    }
}
