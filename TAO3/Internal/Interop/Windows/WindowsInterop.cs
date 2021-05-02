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
        public IToastNotifier ToastNotifier { get; }

        private WindowsInterop(WindowsKeyboardHook keyboardHook, WindowsClipboard clipboard, WindowsToastNotifier windowsToastNotifier)
        {
            KeyboardHook = keyboardHook;
            Clipboard = clipboard;
            ToastNotifier = windowsToastNotifier;
        }

        internal static WindowsInterop Create()
        {
            Task.Run(() => AvaloniaApp.Start());
            AvaloniaApp avaloniaApp = AvaloniaApp.Current;

            return new WindowsInterop(
                avaloniaApp.KeyboardHook,
                new WindowsClipboard(avaloniaApp),
                new WindowsToastNotifier());
        }
    }
}
