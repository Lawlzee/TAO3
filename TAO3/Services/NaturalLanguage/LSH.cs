﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.NaturalLanguage
{
    //https://nickgrattandatascience.wordpress.com/2017/12/31/lsh-for-finding-similar-documents-from-a-large-number-of-documents-in-c/
    public class LSH
    {
        private static readonly uint _prime = 1174247;

        private readonly uint[,] _minHashes;
        private readonly int _bandCount;
        private readonly int _hasFunctionCount;
        private readonly int _hashPerBand;
        private readonly int _wordCount;
        private readonly List<Dictionary<uint, List<int>>> _bands;

        public LSH(uint[,] minHashes, int hashPerBand)
        {
            _minHashes = minHashes;
            _hasFunctionCount = minHashes.GetUpperBound(1) + 1;
            _wordCount = minHashes.GetUpperBound(0) + 1;
            _hashPerBand = hashPerBand;
            _bandCount = _hasFunctionCount / hashPerBand;
            _bands = FillBuckets();
        }

        private List<Dictionary<uint, List<int>>> FillBuckets()
        {
            List<Dictionary<uint, List<int>>> bands = new List<Dictionary<uint, List<int>>>();

            for (int startHashIndex = 0; startHashIndex < _bandCount; startHashIndex += _hashPerBand)
            {
                Dictionary<uint, List<int>> band = new Dictionary<uint, List<int>>();
                for (int wordIndex = 0; wordIndex < _wordCount; wordIndex++)
                {
                    uint hash = 0;
                    for (int hashIndex = startHashIndex; hashIndex < startHashIndex + _hashPerBand; hashIndex++)
                    {
                        hash = unchecked(hash * _prime + _minHashes[wordIndex, hashIndex]);
                    }

                    if (!band.ContainsKey(hash))
                    {
                        band.Add(hash, new List<int>());
                    }
                    band[hash].Add(wordIndex);
                }

                bands.Add(band);
            }

            return bands;
        }

        public List<int> GetNearest(uint[] minHash)
        {
            return GetMatches()
                .Distinct()
                .ToList();

            IEnumerable<int> GetMatches()
            {
                int bandIndex = 0;
                for (int startHashIndex = 0; startHashIndex < _bandCount; startHashIndex += _hashPerBand)
                {
                    uint hash = 0;
                    for (int hashIndex = startHashIndex; hashIndex < startHashIndex + _hashPerBand; hashIndex++)
                    {
                        hash = unchecked(hash * _prime + minHash[hashIndex]);
                    }

                    Dictionary<uint, List<int>> band = _bands[bandIndex];

                    List<int>? bucket = band.GetValueOrDefault(hash);
                    if (bucket != null)
                    {
                        foreach (int i in bucket)
                        {
                            yield return i;
                        }
                    }
                    bandIndex++;
                }
            }

        }
    }
}
