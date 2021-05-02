using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.Threading;
using TAO3.Internal.Interop;
using WindowsHook;

namespace TAO3.Internal.Interop.Windows
{
    internal class AvaloniaApp : Application
    {
        private readonly static Semaphore _lock = new Semaphore(0, 1);

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
