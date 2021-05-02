﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Interop.Windows
{
    internal class WindowsClipboard : IClipboard
    {
        private readonly AvaloniaApp _avaloniaApp;

        public WindowsClipboard(AvaloniaApp avaloniaApp)
        {
            _avaloniaApp = avaloniaApp;
        }

        public Task ClearAsync()
        {
            return _avaloniaApp.Clipboard.ClearAsync();
        }

        public Task<string> GetTextAsync()
        {
            return _avaloniaApp.Clipboard.GetTextAsync();
        }

        public Task SetTextAsync(string text)
        {
            return _avaloniaApp.Clipboard.SetTextAsync(text);
        }
    }
}