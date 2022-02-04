using Microsoft.CodeAnalysis;

namespace TAO3.Converters.CSharp;

[Flags]
public enum CSharpModifiers
{
    None = 0,
    @abstract = 1 << 0,
    @async = 1 << 1,
    @const = 1 << 2,
    @extern = 1 << 3,
    @in = 1 << 4,
    @internal = 1 << 5,
    @new = 1 << 6,
    @out = 1 << 7,
    @override = 1 << 8,
    @params = 1 << 9,
    @private = 1 << 10,
    @protected = 1 << 11,
    @public = 1 << 12,
    @readonly = 1 << 13,
    @ref = 1 << 14,
    @sealed = 1 << 15,
    @static = 1 << 16,
    @unsafe = 1 << 17,
    @virtual = 1 << 18,
    @volatile = 1 << 19
}

public static class CSharpModifiersHelper
{
    public static CSharpModifiers Parse(SyntaxTokenList tokens)
    {
        CSharpModifiers result = CSharpModifiers.None;

        foreach (SyntaxToken token in tokens)
        {
            if (Enum.TryParse(token.ToString(), out CSharpModifiers modifier))
            {
                result |= modifier;
            }
        }

        return result;
    }
}
