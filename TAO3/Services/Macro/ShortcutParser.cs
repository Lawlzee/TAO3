using System.Text.RegularExpressions;
using WindowsHook;

namespace TAO3.Macro;

internal static class ShortcutParser
{
    internal static Keys Parse(string shortcut)
    {
        string[] parts = shortcut.Split("+", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .Select(Substitute)
            .ToArray();

        if (parts.Length == 0)
        {
            throw new ArgumentException(nameof(shortcut));
        }

        return (Keys)parts
            .Select(x => Enum.Parse<Keys>(x, ignoreCase: true))
            .Distinct()
            .Sum(x => (int)x);
    }

    private static string Substitute(string shortcutPart)
    {
        if (shortcutPart.Equals("CTRL", StringComparison.OrdinalIgnoreCase))
        {
            return Keys.Control.ToString();
        }

        return Regex.Replace(shortcutPart, @"^\d$", "D$0");
    }
}
