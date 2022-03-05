using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;

namespace TAO3.Avalonia;

public interface IAvaloniaService : IDisposable
{
    AvaloniaApp Application { get; }

    bool IsApplicationStarted { get; }
    IObservable<Unit> ApplicationStarted { get; }
    IObservable<Unit> ApplicationStoped { get; }

    Task DispatchAsync(Action action);
    Task<T> DispatchAsync<T>(Func<T> action);

    Task<TabItem> AddTabAsync(Func<TabItem> tabFactory);
    Task<MenuItem> AddMenuItemAsync(Func<MenuItem> menuItemFactory);

    Task RemoveTabAsync(TabItem tab);
    Task RemoveMenuItemAsync(MenuItem menuItem);
}

internal class AvaloniaService : IAvaloniaService
{
    private readonly object _lock = new object();
    private AvaloniaApp? _application;
    public bool IsApplicationStarted => _application != null;
    private Subject<Unit> _applicationStarted;
    public IObservable<Unit> ApplicationStarted => _applicationStarted;
    private Subject<Unit> _applicationStoped;
    public IObservable<Unit> ApplicationStoped => _applicationStoped;


    public AvaloniaService()
    {
        _applicationStarted = new Subject<Unit>();
        _applicationStoped = new Subject<Unit>();
    }

    public AvaloniaApp Application
    {
        get
        {
            if (_application != null)
            {
                return _application;
            }

            lock (_lock)
            {
                if (_application != null)
                {
                    return _application;
                }

                return CreateApplication();
            }
        }
    }

    private AvaloniaApp CreateApplication()
    {
        Semaphore initialisationLock = new Semaphore(0, 1);
        _ = Task.Run(() =>
            AppBuilder.Configure(() =>
            {
                _application = new AvaloniaApp(initialisationLock);
                return _application;
            })
            .UsePlatformDetect()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(Array.Empty<string>()));

        initialisationLock.WaitOne();
        _applicationStarted.OnNext(Unit.Default);

        _application!.ApplicationLifetime.Exit += (o, e) =>
        {
            _application = null;
            _applicationStoped.OnNext(Unit.Default);
        };
        return _application;
    }

    public Task DispatchAsync(Action action)
    {
        return Application.UIThread.InvokeAsync(action);
    }

    public Task<T> DispatchAsync<T>(Func<T> action)
    {
        return Application.UIThread.InvokeAsync(action);
    }

    public Task<MenuItem> AddMenuItemAsync(Func<MenuItem> menuItemFactory)
    {
        return DispatchAsync(() =>
        {
            MenuItem menuItem = menuItemFactory();
            Application.Menu.Items = Application.Menu.Items.OfType<IControl>().Append(menuItem).ToList();
            return menuItem;
        });
    }

    public Task<TabItem> AddTabAsync(Func<TabItem> tabFactory)
    {
        return DispatchAsync(() =>
        {
            TabItem tabItem = tabFactory();
            Application.TabControl.Items = Application.TabControl.Items.OfType<IControl>().Append(tabItem).ToList();
            return tabItem;
        });
    }

    public Task RemoveMenuItemAsync(MenuItem menuItem)
    {
        return DispatchAsync(() =>
        {
            Application.Menu.Items = Application.Menu.Items.OfType<IControl>().Where(x => x != menuItem).ToList();
        });
    }

    public Task RemoveTabAsync(TabItem tab)
    {
        return DispatchAsync(() =>
        {
            Application.TabControl.Items = Application.TabControl.Items.OfType<IControl>().Where(x => x != tab).ToList();
        });
    }

    public void Dispose()
    {
        _application?.ApplicationLifetime.Shutdown();
        _applicationStarted.OnCompleted();
        _applicationStoped.OnCompleted();
    }
}

public class AvaloniaApp : Application
{
    private readonly Semaphore _initialisationLock;
    public new IClassicDesktopStyleApplicationLifetime ApplicationLifetime => (IClassicDesktopStyleApplicationLifetime)base.ApplicationLifetime!;
    public Window MainWindow => ApplicationLifetime.MainWindow;
    public Menu Menu { get; private set; } = null!;
    public TabControl TabControl { get; private set; } = null!;
    public Dispatcher UIThread => Dispatcher.UIThread;

    internal AvaloniaApp(Semaphore initialisationLock)
    {
        _initialisationLock = initialisationLock;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Styles.Add(new FluentTheme(new Uri("avares://MinimalAvalonia")) { Mode = FluentThemeMode.Light });
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DockPanel dockPanel = new DockPanel
            {
                LastChildFill = true
            };

            Menu = new Menu
            {
                Items = new List<IControl>
                {
                    new MenuItem
                    {
                        Header = "Open file"//,
                        //Command = ReactiveCommand.Create(() => { })
                    }
                }
            };

            DockPanel.SetDock(Menu, Dock.Top);

            TabControl = new TabControl
            {
                TabStripPlacement = Dock.Left,
                Items = new List<IControl>()
            };

            dockPanel.Children.Add(Menu);
            dockPanel.Children.Add(TabControl);

            desktop.MainWindow = new Window
            {
                Title = "TAO3",
                Content = dockPanel
            };
        }
        _initialisationLock.Release();
    }


}
