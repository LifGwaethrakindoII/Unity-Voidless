using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===========================================================================
**
** Class:  FuzzyMatcher
**
** Purpose: Fuzzy-Matching class. Used for matching different words. It can
** be used either as a static class or an instance.
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
    public class FuzzyMatcher
    {
        private int _maxDistance;
        private List<string> _knownWords;

        /// <summary>Gets and Sets maxDistance property.</summary>
        public int maxDistance
        {
            get { return _maxDistance; }
            set { _maxDistance = Mathf.Max(value, 0); }
        }

        /// <summary>Gets and Sets knownWords property.</summary>
        public List<string> knownWords
        {
            get { return _knownWords; }
            set { _knownWords = value; }
        }

        /// <summary>FuzzyMatcher's constructor.</summary>
        /// <param name="_maxDistance">Max Margin of Error. 2 by default.</param>
        /// <param name="_words">Initial knowledge base. Null by default.</param>
        public FuzzyMatcher(int _maxDistance = 2, IEnumerable<string> _words = null)
        {
            maxDistance = _maxDistance;
            knownWords = new List<string>();
            AddKnownWords(_words);
        }

        /// <summary>Adds words into the knowledge base.</summary>
        /// <param name="_words">Words to add.</param>
        public void AddKnownWords(IEnumerable<string> _words)
        {
            knownWords.AddRange(_words);
        }

        /// <summary>From a set of words, find the closest one to given input.</summary>
        /// <param name="input">Input String.</param>
        /// <returns>Closes match from the valid words. None will be returned if no word was close to input.</returns>
        public string FindClosestMatch(string input)
        {
            string closestMatch = string.Empty;
            int minDistance = int.MaxValue;

            foreach(string word in knownWords)
            {
                int distance = LevenshteinDistance(input.ToLower(), word.ToLower());
                if (distance < minDistance && distance <= maxDistance)
                {
                    minDistance = distance;
                    closestMatch = word;
                }
            }

            return closestMatch; // Returns null if no match is found within maxDistance
        }

        /// <summary>String metric for measuring the difference between 2 strings.</summary>
        /// <param name="a">String A.</param>
        /// <param name="b">String B.</param>
        /// <returns>Difference between 2 strings.</returns>
        public static int LevenshteinDistance(string a, string b)
        {
            int[,] matrix = new int[a.Length +1, b.Length + 1];

            for(int i = 0; i <= a.Length; i++) matrix[i, 0] = i;
            for(int j = 0; j <= b.Length; j++) matrix[0, j] = j;
        
            for(int i = 1; i <= a.Length; i++)
            {
                for(int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    matrix[i, j] = Mathf.Min(
                        Mathf.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }

            return matrix[a.Length, b.Length];
        }

        /// <summary>From a set of words, find the closest one to given input.</summary>
        /// <param name="input">Input String.</param>
        /// <param name="maxDistance">Margin error [2 characters by default].</param>
        /// <param name="validWords">Set of valid words.</param>
        /// <returns>Closes match from the valid words. None will be returned if no word was close to input.</returns>
        public static string FindClosestMatch(string input, int maxDistance = 2, params string[] validWords)
        {
            string closestMatch = string.Empty;
            int minDistance = int.MaxValue;

            foreach(string word in validWords)
            {
                int distance = LevenshteinDistance(input.ToLower(), word.ToLower());
                if (distance < minDistance && distance <= maxDistance)
                {
                    minDistance = distance;
                    closestMatch = word;
                }
            }

            return closestMatch; // Returns null if no match is found within maxDistance
        }
    }
}