namespace YouTube.Downloader.Extensions
{
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        internal static IEnumerable<T> ToEnumerable<T>(T item)
        {
            yield return item;
        }
    }
}