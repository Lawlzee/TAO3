using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TAO3.Clipboard;
using TAO3.Keyboard;
using static TAO3.Internal.Windows.User32;

namespace TAO3.Windows
{
    public interface IWindowsService : IDisposable
    {
        IKeyboardService Keyboard { get; }
        IClipboardService Clipboard { get; }
    }

    public class WindowsService : IWindowsService
    {
        private readonly WndProc _wndProc;
        private WNDCLASSEX _windowsClass;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public IKeyboardService Keyboard { get; }
        public IClipboardService Clipboard { get; }

        public WindowsService()
        {
            var clipboard = new WindowsClipboard();
            Clipboard = clipboard;

            _cancellationTokenSource = new CancellationTokenSource();
            _wndProc = WndProc;

            SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
            IntPtr handle = IntPtr.Zero;
            IKeyboardService keyboard = null!;

            _ = Task.Run(() =>
            {
                _windowsClass = new WNDCLASSEX
                {
                    cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                    lpfnWndProc = _wndProc,
                    hInstance = GetModuleHandle(null!),
                    lpszClassName = "TAO3EventLoop" + Guid.NewGuid()
                };

                ushort atom = RegisterClassEx(ref _windowsClass);

                handle = CreateWindowEx(0, atom, null!, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                SetClipboardViewer(handle);

                keyboard = new WindowsKeyboardService();

                semaphore.Release();

                while (!_cancellationTokenSource.Token.IsCancellationRequested
                    && GetMessage(out MSG msg, IntPtr.Zero, 0, 0) != 0)
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }

                keyboard.Dispose();

            }, _cancellationTokenSource.Token);

            semaphore.Wait();
            semaphore.Dispose();

            Keyboard = keyboard;
            

            IntPtr WndProc(IntPtr hWnd, WindowsMessages msg, IntPtr wParam, IntPtr lParam)
            {
                switch (msg)
                {
                    case WindowsMessages.DRAWCLIPBOARD:
                        clipboard.Subject.OnNext(Unit.Default);
                        break;
                }

                return DefWindowProc(hWnd, msg, wParam, lParam);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
