using System.CommandLine;
using System.Reflection;

namespace TAO3.Internal.Extensions;

internal static class CommandExtensions
{
    private static readonly FieldInfo _subcommandsField = typeof(Command).GetField("_subcommands", BindingFlags.NonPublic | BindingFlags.Instance)!;

    public static void AddAliases(this Command command, IEnumerable<string> aliases)
    {
        foreach (var alias in aliases)
        {
            command.AddAlias(alias);
        }
    }

    public static void RemoveSubCommand(this Command command, Command subCommand)
    {
        List<Command>? subcommands = (List<Command>?)_subcommandsField.GetValue(command);
        subcommands?.Remove(subCommand);
    }
}
