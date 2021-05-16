using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Services.Clipboard;
using TAO3.Services.Keyboard;
using TAO3.Services.Toast;

namespace TAO3.Services.Avalonia
{
    internal class WindowsInterop
    {
        public IKeyboardService Keyboard { get; }
        public IClipboardService Clipboard { get; }

        private WindowsInterop(WindowsKeyboardService keyboardHook, WindowsClipboard clipboard)
        {
            Keyboard = keyboardHook;
            Clipboard = clipboard;
        }

        internal static WindowsInterop Create()
        {
            Task.Run(() => AvaloniaApp.Start());
            AvaloniaApp avaloniaApp = AvaloniaApp.Current;

            return new WindowsInterop(
                avaloniaApp.KeyboardHook,
                new WindowsClipboard(avaloniaApp));
        }
    }
}
