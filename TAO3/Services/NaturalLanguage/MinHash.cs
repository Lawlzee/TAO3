namespace TAO3.NaturalLanguage;

//https://nickgrattandatascience.wordpress.com/2013/11/12/minhash-implementation-in-c/
public class MinHash
{
    private readonly Func<int, uint>[] _hashFunctions;
    private readonly int _numHashFunctions;

    // Constructor passed universe size and number of hash functions
    public MinHash(int hashFunctionCount, int hashBitSize, Random random)
    {
        _numHashFunctions = hashFunctionCount;
        _hashFunctions = GenerateHashFunctions(hashBitSize, random);
    }

    // Generates the Universal Random Hash functions
    // http://en.wikipedia.org/wiki/Universal_hashing
    private Func<int, uint>[] GenerateHashFunctions(int u, Random random)
    {
        Func<int, uint>[] hashFunctions = new Func<int, uint>[_numHashFunctions];

        for (int i = 0; i < _numHashFunctions; i++)
        {
            uint a = 0;
            // parameter a is an odd positive
            while (a % 1 == 1 || a == 0)
            {
                a = (uint)random.Next();
            }

            uint b = 0;
            int maxb = 1 << u;
            // parameter b must be greater than zero and less than universe size
            while (b <= 0)
            {
                b = (uint)random.Next(maxb);
            }

            hashFunctions[i] = x => QHash(x, a, b, u);
        }

        return hashFunctions;
    }

    // Universal hash function with two parameters a and b, and universe size in bits
    private static uint QHash(int x, uint a, uint b, int u)
    {
        return a * (uint)x + b >> 32 - u;
    }

    // Returns the list of min hashes for the given set of word Ids
    public uint[] GetMinHash(int[] shingleHashes)
    {
        uint[] minHashes = new uint[_numHashFunctions];
        for (int h = 0; h < _numHashFunctions; h++)
        {
            minHashes[h] = int.MaxValue;
        }
        foreach (int id in shingleHashes)
        {
            for (int h = 0; h < _numHashFunctions; h++)
            {
                uint hash = _hashFunctions[h](id);
                minHashes[h] = Math.Min(minHashes[h], hash);
            }
        }
        return minHashes;
    }
}
