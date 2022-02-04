using WindowsHook;

namespace TAO3.Keyboard;

public interface IKeyboardService : IDisposable
{
    void RegisterOnKeyPressed(Keys shortcut, Action onPressed);
    bool UnRegisterOnKeyPressed(Keys shortcut);
}
