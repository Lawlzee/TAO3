using Fastenshtein;
using rm.Trie;
using TAO3.Extensions;

namespace TAO3.NaturalLanguage;

public interface ILanguageDictionary
{
    List<WordMatched> GetSimilarWords(string word);
    IEnumerable<string> GetWords(string prefix);
    bool ContainsWord(string word);
}

public record WordMatched(
    string Word,
    double SimilarityIndex);

public record LanguageDictionaryOptions
{
    public int Seed { get; init; } = 0;
    public int HashBitSize { get; init; } = 30;

    public int BandCount { get; init; } = 40;
    public int RowPerBand { get; init; } = 3;

    public int ShingleSize { get; init; } = 2;

    public double MinSimilarityIndex { get; init; } = 0.75;

    public Func<string, string> NormaliseWord { get; init; } = x => x.RemoveDiacritics().ToUpper();
}

public class LanguageDictionary : ILanguageDictionary
{
    private readonly IList<string> _words;
    private readonly TrieMap<string> _wordsPrefixTree;
    private readonly LSH _lsh;
    private readonly MinHash _minHash;
    private readonly LanguageDictionaryOptions _options;

    public LanguageDictionary(IList<string> words, LanguageDictionaryOptions? options = null)
    {
        _words = words;
        _wordsPrefixTree = new TrieMap<string>();
        _options = options ?? new LanguageDictionaryOptions();

        int hashFunctionCount = _options.BandCount * _options.RowPerBand;
        _minHash = new MinHash(hashFunctionCount, _options.HashBitSize, new Random(_options.Seed));

        uint[,] minhashes = new uint[words.Count, hashFunctionCount];
        for (int rowIndex = 0; rowIndex < words.Count; rowIndex++)
        {
            string word = words[rowIndex];
            string normalisedWord = _options.NormaliseWord(words[rowIndex]);
            _wordsPrefixTree.Add(normalisedWord, word);

            int[] shingleHashes = Shingler.Shingle(normalisedWord, _options.ShingleSize);
            uint[] minHash = _minHash.GetMinHash(shingleHashes);
            for (int i = 0; i < hashFunctionCount; i++)
            {
                minhashes[rowIndex, i] = minHash[i];
            }
        }

        _lsh = new LSH(minhashes, _options.RowPerBand);
    }

    public List<WordMatched> GetSimilarWords(string word)
    {
        string normalisedWord = _options.NormaliseWord(word);
        int[] shingleHash = Shingler.Shingle(normalisedWord, _options.ShingleSize);
        uint[] valueMinHash = _minHash.GetMinHash(shingleHash);

        Levenshtein levenshtein = new Levenshtein(normalisedWord);

        List<WordMatched> nearestValues = _lsh.GetNearest(valueMinHash)
            .Select(i =>
            {
                string word = _words[i];
                string normalisedMatch = _options.NormaliseWord(word);

                return new WordMatched(
                    word,
                    normalisedWord.Length == 0 && normalisedMatch.Length == 0
                        ? 0
                        : 1 - levenshtein.DistanceFrom(normalisedMatch) / (double)Math.Max(normalisedMatch.Length, normalisedWord.Length));
            })
            .Where(x => x.SimilarityIndex >= _options.MinSimilarityIndex)
            .OrderByDescending(x => x.SimilarityIndex)
            .ToList();

        return nearestValues;
    }

    public IEnumerable<string> GetWords(string prefix)
    {
        string normalisedPrefix = _options.NormaliseWord(prefix);
        return _wordsPrefixTree.ValuesBy(normalisedPrefix);
    }

    public bool ContainsWord(string word)
    {
        string normalisedWord = _options.NormaliseWord(word);
        return _wordsPrefixTree.HasKey(normalisedWord);
    }
}
