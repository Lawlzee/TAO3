using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Clipboard;
using TAO3.Keyboard;
using TAO3.Toast;

namespace TAO3.Avalonia
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
