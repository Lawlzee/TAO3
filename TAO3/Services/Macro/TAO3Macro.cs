using WindowsHook;

namespace TAO3.Macro;

public record TAO3Macro(
    string Name,
    Keys Shortcut,
    string Code,
    string? TargetKernelName = null,
    bool ToastNotificationOnRun = true)
{
    public TAO3Macro(
        string name,
        string shortcut,
        string code,
        string? targetKernelName = null,
        bool toastNotificationOnRun = true)
        : this(name, ShortcutParser.Parse(shortcut), code, targetKernelName, toastNotificationOnRun)
    {
    }
}
