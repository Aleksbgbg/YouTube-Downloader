namespace YouTube.Downloader.Extensions
{
    using System.Collections.Generic;

    internal static class ObjectExtensions
    {
        internal static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}