namespace TAO3.NaturalLanguage;

public static class Shingler
{
    public static int[] Shingle(string str, int shingleSize)
    {
        if (str.Length == 0)
        {
            return Array.Empty<int>();
        }

        if (str.Length < shingleSize)
        {
            HashCode hashcode = new();
            for (int i = 0; i < str.Length; i++)
            {
                hashcode.Add(str[i]);
            }

            return new int[] { hashcode.ToHashCode() };
        }

        int[] result = new int[str.Length - shingleSize + 1];

        for (int i = 0; i <= str.Length - shingleSize; i++)
        {
            HashCode hashcode = new();
            for (int j = 0; j < shingleSize; j++)
            {
                hashcode.Add(str[i + j]);
            }
            result[i] = hashcode.ToHashCode();
        }

        return result;
    }
}
