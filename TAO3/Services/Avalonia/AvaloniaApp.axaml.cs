using Avalonia;
using Avalonia.Markup.Xaml;
using System;
using System.Threading;
using TAO3.Services.Keyboard;

namespace TAO3.Services.Avalonia
{
    internal class AvaloniaApp : Application
    {
        private readonly static Semaphore _lock = new Semaphore(0, 1);

#nullable disable
        internal WindowsKeyboardService KeyboardHook { get; private set; }
#nullable enable
        internal static new AvaloniaApp Current
        {
            get
            {
                try
                {
                    _lock.WaitOne();
                    return (AvaloniaApp)Application.Current;
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();
            KeyboardHook = new WindowsKeyboardService();
            _lock.Release();
        }

        public static void Start()
        {
            AppBuilder.Configure<AvaloniaApp>()
                .UsePlatformDetect()
                .LogToTrace()
                .StartWithClassicDesktopLifetime(Array.Empty<string>());
        }
    }
}
