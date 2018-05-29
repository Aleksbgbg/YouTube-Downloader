namespace YouTube.Downloader.Extensions
{
    using System;

    internal static class StringExtensions
    {
        internal static int LevenshteinDistance(this string source, string target)
        {
            if (source.Length == 0)
            {
                return target.Length;
            }

            if (target.Length == 0)
            {
                return source.Length;
            }

            int[,] distances = new int[source.Length + 1, target.Length + 1];

            for (int column = -1; column < source.Length; distances[++column, 0] = column)
            {
            }

            for (int row = -1; row < target.Length; distances[0, ++row] = row)
            {
            }

            for (int column = 1; column <= source.Length; ++column)
            {
                for (int row = 1; row <= target.Length; ++row)
                {
                    int previousColumn = column - 1;
                    int previousRow = row - 1;

                    distances[column, row] = Math.Min(Math.Min(distances[previousColumn, row] + 1, distances[column, previousRow] + 1),
                                                      distances[previousColumn, previousRow] + (target[previousRow] == source[previousColumn] ? 0 : 1));
                }
            }

            return distances[source.Length, target.Length];
        }

        internal static double PercentageSimilarity(this string source, string target)
        {
            if (source.Length == 0 || target.Length == 0)
            {
                return 0;
            }

            if (source == target)
            {
                return 1;
            }

            int levenshteinDistance = source.LevenshteinDistance(target);

            return 1 - (double)levenshteinDistance / Math.Max(source.Length, target.Length);
        }
    }
}