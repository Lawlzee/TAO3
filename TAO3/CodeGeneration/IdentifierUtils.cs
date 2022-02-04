namespace TAO3.CodeGeneration;

internal static class IdentifierUtils
{
    internal static string ToCSharpIdentifier(string str)
    {
        StringBuilder? sb = new StringBuilder(str.Length);
        bool shouldUpper = true;

        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(shouldUpper ? char.ToUpper(c) : c);
                shouldUpper = false;
            }
            else
            {
                shouldUpper = true;
            }
        }

        return sb.ToString();
    }

    internal static string GetUniqueIdentifier(string identifier, HashSet<string> usedIdentifiers)
    {
        for (int i = 2; true; i++)
        {
            string newIdentifier = identifier + i;
            if (!usedIdentifiers.Contains(newIdentifier))
            {
                return newIdentifier;
            }
        }
    }
}
