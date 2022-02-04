using WindowsHook;

namespace TAO3.Keyboard;

//Make thread safe?
internal class WindowsKeyboardService : IKeyboardService
{
    private Keys _lastKeyData;
    private readonly Dictionary<Keys, Action> _shortcuts;
    private readonly IKeyboardMouseEvents _keyboardMouseEvents;

    public WindowsKeyboardService()
    {
        _keyboardMouseEvents = Hook.GlobalEvents();
        _shortcuts = new Dictionary<Keys, Action>();
        _keyboardMouseEvents.KeyDown += OnKeydown;
        _keyboardMouseEvents.KeyUp += OnKeyUp;
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        _lastKeyData -= e.KeyData;
    }

    private void OnKeydown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == _lastKeyData)
        {
            return;
        }

        if (_shortcuts.TryGetValue(e.KeyData, out Action? onClick))
        {
            onClick?.Invoke();
        }

        _lastKeyData = e.KeyData;
    }

    public void RegisterOnKeyPressed(Keys shortcut, Action onPressed)
    {
        _shortcuts.Remove(shortcut);
        _shortcuts[shortcut] = onPressed;
    }

    public void Dispose()
    {
        _keyboardMouseEvents.Dispose();
    }

    public bool UnRegisterOnKeyPressed(Keys shortcut)
    {
        return _shortcuts.Remove(shortcut);
    }
}
