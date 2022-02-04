namespace TAO3.Internal.Types;

internal static class NumberHelper
{
    private readonly static Dictionary<Type, string> _prefixes = new Dictionary<Type, string>()
    {
        [typeof(long)] = "L",
        [typeof(double)] = "d",
        [typeof(float)] = "f",
        [typeof(decimal)] = "m",
        [typeof(uint)] = "u",
        [typeof(ulong)] = "UL"
    };

    public static string? GetNumberSuffix(Type numberType)
    {
        if (_prefixes.ContainsKey(numberType))
        {
            return _prefixes[numberType];
        }
        return null;
    }
}
