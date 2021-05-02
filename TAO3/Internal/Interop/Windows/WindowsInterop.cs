using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Interop.Windows
{
    internal class WindowsInterop : IInteropOS
    {
        public IKeyboardHook KeyboardHook { get; }
        public IClipboard Clipboard { get; }

        private WindowsInterop(WindowsKeyboardHook keyboardHook, WindowsClipboard clipboard)
        {
            KeyboardHook = keyboardHook;
            Clipboard = clipboard;
        }

        internal static WindowsInterop Create()
        {
            Task.Run(() => AvaloniaApp.Start());
            AvaloniaApp avaloniaApp = AvaloniaApp.Current;
            return new WindowsInterop(
                new WindowsKeyboardHook(),
                new WindowsClipboard(avaloniaApp));
        }
    }
}
